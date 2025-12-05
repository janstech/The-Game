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

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameOver = false;
    public GameObject makerText;
    public GameObject gameOverPanel;
    public int totalOrbs = 10;
    private int collectedOrbs = 0;
    public VictoryUI victoryUI;
    public TextMeshProUGUI orbCounterInfo;
    public CanvasGroup orbCounterGroup; 
    public int difficulty = 1; // 1 = normaali, 2 = vaikea, jne.
    void Start()
    {
        makerText.SetActive(false);
        UpdateOrbCounter(); // Päivitettään Orb-laskuri
        ApplyDifficultySettings(); // Päivitetään vaikeustasot
        StartCoroutine(FadeInCounter(3.5f)); // x sekunnin fade-in

    }

    void Awake()
    {

        // Singleton: vain yksi GameManager pysyy aktiivisena
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Säilyy scenejen välillä - Restart-nappi toimi vain kerran tämän kanssa
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (victoryUI == null)
        {
            victoryUI = FindAnyObjectByType<VictoryUI>();
        }   
    }

    public void ApplyDifficultySettings()
    {
        switch (difficulty)
        {
            case 1: totalOrbs = 5; break;   // Just Chillin’  
            case 2: totalOrbs = 15; break;  // Bring It On
            case 3: totalOrbs = 20; break;  // No Mercy
            case 4: totalOrbs = 30; break;  // Good Luck, Loser!
        }

        OrbSpawner orbSpawner = FindAnyObjectByType<OrbSpawner>();
        if (orbSpawner != null)
        {
            orbSpawner.SetMaxOrbs(totalOrbs);
        }
        else
        {
            Debug.LogWarning("OrbSpawner ei löytynyt GameManagerista!");
        }

        Debug.Log($"[GameManager] Vaikeustaso {difficulty} -> totalOrbs = {totalOrbs}");
        UpdateOrbCounter(); // Päivitetään laskuri vaikeustason mukaan

    }


    public void CollectOrb()
    {
        collectedOrbs++;
        UpdateOrbCounter();

        Debug.Log($"Collected {collectedOrbs} / {totalOrbs}");


        if (collectedOrbs >= totalOrbs)
        {
            Victory();
        }
    }

    void UpdateOrbCounter()
    {
        if (orbCounterInfo != null)
        {
            orbCounterInfo.text = $"Orbs: {collectedOrbs} / {totalOrbs}";
        }
    }


    public void Victory()
    {
        Time.timeScale = 0f; // Pysäyttää pelin
        Debug.Log("Voitit pelin!");
        victoryUI.ShowVictory();


        // Voit näyttää erillisen Victory UI:n
        // victoryPanel.SetActive(true);
    }


    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("GAME OVER!");
        makerText.SetActive(true);
        ShowRestartButtonDelayed(2.5f); // Säädä aika vastaamaan animaatiota
    }


    public void RestartGame()
    {
        isGameOver = false;
        Debug.Log("Restart painettu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Ladataan nykyinen scene uudelleen
        StartCoroutine(RestoreSensitivityDelayed(0.2f));

    }

    private IEnumerator RestoreSensitivityDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        SettingsManager settings = FindAnyObjectByType<SettingsManager>();
        if (settings != null)
        {
            Debug.Log("Herkkys palautettu restartin jälkeen.");

            int index = PlayerPrefs.GetInt("SensitivityIndex", 1);
            settings.OnSensitivityChanged(index);
            Debug.Log("Herkkys palautettu restartin jälkeen.");
        }
        else
        {
            Debug.LogWarning("SettingsManager ei löytynyt restartin jälkeen!");
        }
    }

    IEnumerator FadeInCounter(float duration)   //  Fade-in-animaatio laskurille
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            orbCounterGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        orbCounterGroup.alpha = 1f;
    }

    public void SetDifficulty(int newDifficulty)
    {
        difficulty = newDifficulty;
    }

    public int GetDifficulty()
    {
        return difficulty;
    }
    
    public void ShowRestartButtonDelayed(float delay)
    {
        StartCoroutine(DelayedRestartButton(delay));
    }


    private IEnumerator DelayedRestartButton(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameOverPanel.SetActive(true);
        makerText.SetActive(true);

        // Hiiren kursori näkyviin vasta sitten
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
