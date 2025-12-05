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
using System.Collections;
using UnityEngine.EventSystems;

public class WeaponController : MonoBehaviour
{
    public Animator animator;
    public AudioSource shotSound;
    public GameObject hitPrefab;     // Viholliseen osuessa syntyvä efekti
    public LineRenderer laserLine;  // Viiva, joka näyttää laserin
    public Transform muzzlePoint;    // Laserin lähtöpiste
    public float laserLength = 150f; // Maksimietäisyys mihin voi ampua
    public float laserDuration = 0.05f; // Kuinka kauan laser näkyy
    public float fireRate = 0.3f; // Tulinopeus sekunteina

    private float nextFire = 0f; // Aika jolloin saa ampua seuraavan kerran
    public LayerMask laserHitMask; // Mihin layereihin laser voi osua
    public bool canShoot = true; // Voiko ampua vai onko estetty



    void Start()
    {
        animator?.ResetTrigger("Shoot");

        if (laserLine != null)
            laserLine.enabled = false; // Piilotetaan viiva alussa


    }

    void Update()

    {
        if (!canShoot) return;

        if (Input.GetKeyDown(KeyCode.L)) // Testi laserin piirtämiseen
        {
            Debug.Log("TEST: Force line draw");
            laserLine.enabled = true;
            laserLine.SetPosition(0, transform.position);
            laserLine.SetPosition(1, transform.position + transform.forward * 10);
        }


        // Estetään ampuminen, jos peli on ohi
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;
    

        // Tarkistetaan onko hiiren kursori UI-elementin päällä
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // Hiiri on UI:n päällä, ei tehdä shoot-metodia
        
        // Hiiren vasemmasta napista laukaisu
        if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
        
        
    }

    void Shoot()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main is NULL! Tarkista että kameralla on MainCamera-tag.");
            return;
        }

        Debug.Log("Shoot() called");
            if (laserLine == null)

            {
                Debug.LogError("laserLine is NULL! Et ole linkittänyt LineRendereria.");
                return;
            }

        animator?.SetTrigger("Shoot"); // Käynnistetään animaatio
        shotSound?.Play();

        // Ray kameran keskeltä
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 direction = Camera.main.transform.forward;

        RaycastHit hit;
        Vector3 endPosition = rayOrigin + direction * laserLength;
        
        // Osuiko johonkin
        if (Physics.Raycast(rayOrigin, direction, out hit, laserLength, laserHitMask))
        {
            endPosition = hit.point;

            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Ground"))
            {
                Debug.Log("Laser osui: " + hit.collider.name);


                //  Hit-efekti vain Enemy-objektille
                if (!hit.collider.CompareTag("PowerUp") && !hit.collider.CompareTag("Orb")) // Älä luo efektiä PowerUpeille/Orbille
                {
                    if (hitPrefab != null)
                    {
                        Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                }
                // Tarkista onko Enemy ja vahingoita
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
                else
                {
                    Monster monster = hit.collider.GetComponent<Monster>();
                    if (monster != null)
                    {
                        monster.TakeDamage(1); // Vähennetään Monsterin healthia
                    }

                    else if (!hit.collider.CompareTag("PowerUp") && !hit.collider.CompareTag("Orb"))
                    {
                        Destroy(hit.collider.gameObject); // Tuhotaan vain, jos ei ole PowerUp tai Orb
                    }
                    else
                    {
                        Debug.Log("Osuttiin powerupiin tai orbiin, ei tuhota sitä.");
                    }

                }

            }
        }

        Debug.Log("Calling StartCoroutine(ShowLaser)");


        // Näytä laser-viiva hetken
        StartCoroutine(ShowLaser(endPosition));
    }

    IEnumerator ShowLaser(Vector3 hitPoint)
    {
        Debug.Log("ShowLaser() started. laserLine is " + (laserLine == null ? "NULL" : "OK"));

        Debug.Log("Laser ON");
        laserLine.enabled = true;
        laserLine.SetPosition(0, muzzlePoint.position); // Lähtee piipusta
        laserLine.SetPosition(1, hitPoint);             // Päättyy osumapisteeseen

        float elapsed = 0f;
        float startWidth = 0.05f;
        float endWidth = 0f;

        // Pienennetään viivan paksuutta kunnes häviää
        while (elapsed < laserDuration)
        {
            float t = elapsed / laserDuration;
            float width = Mathf.Lerp(startWidth, endWidth, t);
            laserLine.startWidth = width;
            laserLine.endWidth = width;

            elapsed += Time.deltaTime;
            yield return null;
        }

        laserLine.enabled = false; // Sammutetaan viiva
    }
}
