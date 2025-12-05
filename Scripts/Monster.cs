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


using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [HideInInspector] public float speed = 5.0f;

    private Animator animator;
    private GameObject player;
    public int health = 5;  // Monsteriin tarvittavat osumat
    private bool isDead = false;

    public AudioClip enemyApproachClip;
    public AudioClip monsterDeathClip;

    private AudioSource audioSource;
    public float alertDistance = 10f;    // Etäisyys milloin ääntä soitetaan
    private float soundCooldown = 5f;   // Äänen toistoväli
    private float lastSoundTime = -10f;

    private NavMeshAgent agent;     // Monster-objektille älyominaisuudet, että se osaa kiertää esteitä ja navigoida kohteeseen

    public float attackRange = 2.0f;    // Hyökkäysetäisyys
    public float attackCooldown = 2.0f; // Hyökkäysten välinen aika
    private float lastAttackTime = -10f;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1f;      // Pysähdytään pienen matkan päähän pelaajasta
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        // Äänet
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = enemyApproachClip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Start()
    {
        Debug.Log("Monster speed: " + speed);
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Testi: pakotettu hyökkäys
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerAttack();
        }

        // Liiku, jos ollaan hyökkäysetäisyyden ulkopuolella
        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.ResetPath();
        }

        // Hyökkäys
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            TriggerAttack();

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }

        // Animaatio: liikkuuko agentti
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }

        // Äänet selän takaa
        if (Time.time - lastSoundTime > soundCooldown)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Vector3 toEnemy = (transform.position - player.transform.position).normalized;
            float dot = Vector3.Dot(player.transform.forward, toEnemy);

            if (distance < alertDistance && dot > 0.3f && enemyApproachClip != null)
            {
                audioSource.Play();
                lastSoundTime = Time.time;
            }
        }
    }



    // Käynnistää hyökkäysanimaation ja resetoi cooldownin
    public void TriggerAttack()
    {
        if (isDead || animator == null) return;

        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }


    void ResumeMovement()
    {
        if (!isDead && agent != null)
        {
            agent.isStopped = false;
        }
    }


    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }

        if (health <= 0)
        {
            Die();
        }
    }

    // Pysäytetään liike, soitetaan animaatio + ääni, poistetaan collider, tuhotaan objekti hetken kuluttua
    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("KillShot");
        }

        if (monsterDeathClip != null)
        {
            AudioSource.PlayClipAtPoint(monsterDeathClip, transform.position);
        }

        // Estä liike
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        GetComponent<Collider>().enabled = false;

        // Tuhotaan objekti x sekunnin kuluttua
        Destroy(gameObject, 4.5f);
    }
}





