/*

                                                                                                                                                                                                                 
                                                                                                              
    ███        ▄█    █▄       ▄████████         ▄██████▄     ▄████████   ▄▄▄▄███▄▄▄▄      ▄████████           
▀█████████▄   ███    ███     ███    ███        ███    ███   ███    ███ ▄██▀▀▀███▀▀▀██▄   ███    ███           
   ▀███▀▀██   ███    ███     ███    █▀         ███    █▀    ███    ███ ███   ███   ███   ███    █▀            
    ███   ▀  ▄███▄▄▄▄███▄▄  ▄███▄▄▄           ▄███          ███    ███ ███   ███   ███  ▄███▄▄▄               
    ███     ▀▀███▀▀▀▀███▀  ▀▀███▀▀▀          ▀▀███ ████▄  ▀███████████ ███   ███   ███ ▀▀███▀▀▀               
    ███       ███    ███     ███    █▄         ███    ███   ███    ███ ███   ███   ███   ███    █▄            
    ███       ███    ███     ███    ███        ███    ███   ███    ███ ███   ███   ███   ███    ███           
   ▄████▀     ███    █▀      ██████████        ████████▀    ███    █▀   ▀█   ███   █▀    ██████████           
                                                                                                              
                                                                                                              
                                                                                                              
                                                                                                          
      ___      __      _____  ___        ________     __        _______    __  ___      ___  ____  ____    ______    
     |"  |    /""\    (\"   \|"  \      /"       )   /""\      /"      \  |" \|"  \    /"  |("  _||_ " |  /    " \   
     ||  |   /    \   |.\\   \    |    (:   \___/   /    \    |:        | ||  |\   \  //  / |   (  ) : | // ____  \  
     |:  |  /' /\  \  |: \.   \\  |     \___  \    /' /\  \   |_____/   ) |:  | \\  \/. ./  (:  |  | . )/  /    ) :) 
  ___|  /  //  __'  \ |.  \    \. |      __/  \\  //  __'  \   //      /  |.  |  \.    //    \\ \__/ //(: (____/ //  
 /  :|_/ )/   /  \\  \|    \    \ |     /" \   :)/   /  \\  \ |:  __   \  /\  |\  \\   /     /\\ __ //\ \        /   
(_______/(___/    \___)\___|\____\)    (_______/(___/    \___)|__|  \___)(__\_|_)  \__/     (__________) \"_____/    
                                                                                                                     
             ___________  __  ___      ___  __          _______     ______    _______    _______                     
            ("     _   ")|" \|"  \    /"  ||" \        /"     "\   /    " \  /"     "\  /"     "\                    
             )__/  \\__/ ||  |\   \  //  / ||  |      (__/\    :) // ____  \(__/\    :)(__/\    :)                   
                \\_ /    |:  | \\  \/. ./  |:  |          / ___/ /  /    ) :)   / ___/     / ___/                    
                |.  |    |.  |  \.    //   |.  |         // \___(: (____/ //   // \___    // \___                    
                \:  |    /\  |\  \\   /    /\  |\       (:  /  "\\        /   (:  /  "\  (:  /  "\                   
                 \__|   (__\_|_)  \__/    (__\_|_)       \_______)\"_____/     \_______)  \_______)        
                   
                                     © 2025 Jan Sarivuo. All Rights Reserved.        
                                                                                                                     

*/
// Enemy-objekti seuraa ja yrittää tuhota pelaajan

using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public Animator animator;
    public int health = 3;
    private bool isDead = false;

    public AudioClip enemyApproachClip;
    public AudioClip enemyDeathClip;
    private AudioSource audioSource;
    public float alertDistance = 10f;
    private float soundCooldown = 5f;
    private float lastSoundTime = -10f;

    private GameObject player;
    private NavMeshAgent agent;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = 1f;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.clip = enemyApproachClip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D-ääni
    }

    void Update()
    {
        if (isDead || player == null) return;

        agent.SetDestination(player.transform.position);

        // Katso kohti pelaajaa (vaakasuunnassa)
        Vector3 lookDirection = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(lookDirection);

        // Animaatio
        if (animator != null)
        {
            animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);
        }

        // Ääni pelaajan selän takaa
        if (Time.time - lastSoundTime > soundCooldown)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Vector3 toEnemy = (transform.position - player.transform.position).normalized;
            float dot = Vector3.Dot(player.transform.forward, toEnemy); // -1 = selän takana

            if (distance < alertDistance && dot > 0.3f && enemyApproachClip != null)
            {
                audioSource.Play();
                lastSoundTime = Time.time;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("KillShot");
        }

        if (enemyDeathClip != null)
        {
            AudioSource.PlayClipAtPoint(enemyDeathClip, transform.position);
        }

        // Estä liike
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        GetComponent<Collider>().enabled = false;



        // Tuhotaan objekti x sekunnin kuluttua
        Destroy(gameObject, 3.9f);
    }
}
