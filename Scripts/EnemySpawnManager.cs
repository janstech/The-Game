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

// Luodaan Enenmy-objekteja satunnaisiin paikkoihin pelialueella

using System.Collections;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public Transform player;
    public float spawnInterval = 6f;
    public float spawnRadius = 30f;
    public float raycastHeight = 50f;
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public float checkRadius = 1f; // varmistetaan ettei spawnaa seinään/esteeseen



 
    void Start()
    {
        StartCoroutine(InitializeSpawner());  
    }

    IEnumerator InitializeSpawner()
    {
        // pieni viive varmistaa, että vaikeustaso ja muut alustukset ehtivät tapahtua ennen pelin alkua
        yield return new WaitForSeconds(0.1f); 
    
        int difficulty = GameManager.Instance.GetDifficulty();  // Haetaan vaikeustaso Game Managerista

        switch (difficulty)                                     // Vaikeustason valinnan vaikutus Enemyn spawningnopeuteen
        {
            case 1: // Just Chillin’
                spawnInterval = 6f;
                break;
            case 2: // Bring It On
                spawnInterval = 4f;
                break;
            case 3: // No Mercy
                spawnInterval = 2f;
                break;
            case 4: // Good Luck, Loser!
                spawnInterval = 1f;
                break;
        }

        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);

        Debug.Log("Spawn interval: " + spawnInterval);



    }



    void SpawnEnemy()
    {
        // Yritetään löytää paikka viholliselle enintään 20 kertaa
        for (int i = 0; i < 20; i++)
        {
            // Lasketaan satunnainen sijainti pelaajan ympäriltä
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                player.position.x + circle.x,
                raycastHeight,
                player.position.z + circle.y
            );
            // Raycast lasketaan alas maahan groundMaskiin
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundMask))
            {
                Vector3 finalPos = hit.point + Vector3.up * 0.5f;
                
                // Tarkistetaan ettei spawnata esteen sisään
                if (Physics.OverlapSphere(finalPos, checkRadius, obstacleMask).Length == 0)
                {
                    GameObject newEnemy = Instantiate(EnemyPrefab, finalPos, Quaternion.identity);
                    Enemy enemyScript = newEnemy.GetComponent<Enemy>();

                    int difficulty = GameManager.Instance.GetDifficulty();
                    switch (difficulty)
                    {
                        // Vaikeustason valinnan vaikutus Enemyn nopeuteen
                        case 1: enemyScript.speed = 2f; break;
                        case 2: enemyScript.speed = 5f; break;
                        case 3: enemyScript.speed = 8f; break;
                        case 4: enemyScript.speed = 11f; break;
                    }



                    Debug.Log("Instantiated enemy speed: " + enemyScript.speed);
                    return;
                }
            }
        }

        Debug.LogWarning("Ei löytynyt tyhjää paikkaa viholliselle.");
    }


}
