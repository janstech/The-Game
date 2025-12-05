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


// Luodaan PowerUp-objekteja satunnaisesti pelialueelle

using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public int maxPowerUps = 3;
    private int currentPowerUps = 0;
    public GameObject powerUpPrefab;
    public Transform player;
    public float spawnInterval = 5f;
    public float spawnRadius = 30f;
    public float raycastHeight = 50f;
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public float checkRadius = 1f; // Tarkistaa ettei osu seinään

    void Start()
    {
        InvokeRepeating(nameof(SpawnPowerUp), 2f, spawnInterval);
    }

    void SpawnPowerUp()
    {
        if (currentPowerUps >= maxPowerUps)
            return;

        for (int i = 0; i < 20; i++) // Yritetään max. 20 kertaa löytää sopiva paikka
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                player.position.x + circle.x,
                raycastHeight,
                player.position.z + circle.y
            );

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundMask))
            {
                // Lasketaan nostokorkeus colliderin koon mukaan
                float lift = 0.5f;
                Collider prefabCollider = powerUpPrefab.GetComponentInChildren<Collider>();

                if (prefabCollider != null)
                {
                    lift = prefabCollider.bounds.extents.y + 1.3f;
                }

                Vector3 finalPos = hit.point + Vector3.up * lift;

                // Tarkistetaan ettei osu seinään tms.
                if (Physics.OverlapSphere(finalPos, checkRadius, obstacleMask).Length == 0)
                {
                    GameObject powerUp = Instantiate(powerUpPrefab, finalPos, Quaternion.identity);
                    currentPowerUps++;

                    // Kun PowerUp tuhoutuu, ilmoitetaan spawnerille
                    powerUp.AddComponent<PowerUpSelfDestruct>().spawner = this;
                    return;
                }
            }
        }

        Debug.LogWarning("Ei löytynyt tyhjää paikkaa PowerUpille.");
    }

    public void PowerUpCollected()
    {
        currentPowerUps = Mathf.Max(0, currentPowerUps - 1);
    }
}

