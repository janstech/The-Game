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

public class OrbSpawner : MonoBehaviour

{
    public GameObject orbPrefab;
    public int maxOrbs = 5;
    public float spawnInterval = 15f;
    public float spawnRadius = 30f;
    public Transform centerPoint; // esim. pelaajan sijainti
    public LayerMask navMeshLayer;

    private int currentOrbs = 0;


    void Start()
    {
        maxOrbs = GameManager.Instance.totalOrbs;
        InvokeRepeating(nameof(SpawnOrb), 2f, spawnInterval);
    }

    void SpawnOrb()
    {
        if (currentOrbs >= maxOrbs) return;

        for (int i = 0; i < 20; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPos = new Vector3(
                centerPoint.position.x + randomCircle.x,
                centerPoint.position.y + 5f,
                centerPoint.position.z + randomCircle.y
            );

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                float orbHeight = 1.0f;  // orbin säde 
                Vector3 spawnPos = hit.position + Vector3.up * orbHeight;

                float checkRadius = 0.6f;
                float checkHeight = 1.0f; // voidaan säätää colliderin mukaan
                Vector3 checkCenter = spawnPos + Vector3.up * (checkHeight / 2f);

                // Tarkistetaan että ympärillä ei ole mitään esteitä
                Collider[] colliders = Physics.OverlapCapsule(
                    spawnPos,
                    spawnPos + Vector3.up * checkHeight,
                    checkRadius
                );

                bool blocked = false;
                foreach (var col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        blocked = true;
                        break;
                    }
                }

                if (blocked)
                    continue;

                // Spawnaa orb turvallisesti
                GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
                currentOrbs++;

                EnergyOrb orbScript = orb.GetComponent<EnergyOrb>();
                if (orbScript != null)
                {
                    orbScript.spawner = this;
                }
                else
                {
                    Debug.LogWarning("EnergyOrb-komponenttia ei löytynyt orbPrefabista!");
                }

                return;
            }
        }

        Debug.LogWarning("Ei löytynyt paikkaa orbille NavMeshiltä.");
    }



    public void SetMaxOrbs(int amount)
    {
        maxOrbs = amount;
    }


    public void OrbCollected()
    {
        currentOrbs = Mathf.Max(0, currentOrbs - 1);
        GameManager.Instance.CollectOrb(); // Tiedottaa GameManagerille

        //totalCollectedOrbs++;

        /*Debug.Log("Orbit kerätty yhteensä: " + totalCollectedOrbs + " / " + goalOrbs);

        if (totalCollectedOrbs >= goalOrbs)                                                 // Logiikka siirretty GameManageriin
        {
            GameManager.Instance.Victory(); // Orbit kerättyä peli menee Win Game-näkymään
            Debug.Log("Kaikki orbit kerätty! Peli päättyy.");
        }*/
    }
}