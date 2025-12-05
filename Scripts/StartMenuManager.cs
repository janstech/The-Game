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
using TMPro;
using System;

public class StartMenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject crosshairObject;
    public GameObject screenTopPanel;
    public TMP_Dropdown difficultyDropdown;
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;



    public string taghideTag = "Crosshair";



    void Start()
    {
        screenTopPanel.SetActive(false);    // Piilotetaan yläpalkki aluksi
        Time.timeScale = 0f; // Pause peli aluksi
        startPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayMusic(menuMusic, 0.2f);



        // Etsitään kaikki tagilla merkityt objektit (tähtäimet) ja piilotetaan ne
        GameObject[] objectshideTag = GameObject.FindGameObjectsWithTag(taghideTag);    // Piilotetaan tähtäinristikko ennen pelin alkua

        foreach (GameObject obj in objectshideTag)
        {
            obj.SetActive(false);
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return; // älä vaihda turhaan
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume; //Säädetään äänenvoimakkuus
        musicSource.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }
    }

    public void ShowHiddenObjects() // Palautetaan piilotettu tagi 
    {
        GameObject[] objectsToShow = GameObject.FindGameObjectsWithTag(taghideTag);

        foreach (GameObject obj in objectsToShow)
        {
            obj.SetActive(true);
        }
    }


    public void OnDifficultyChanged(int index)          // Vaikeustaso
    {
        GameManager.Instance.SetDifficulty(index + 1); // Difficulty 1–4
        GameManager.Instance.ApplyDifficultySettings();
        Debug.Log("Vaikeustaso asetettu: " + (index + 1));
    }


    public void StartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;   // Lukitaan kursori peliin
        Debug.Log("StartGame called");
        startPanel.SetActive(false);
        Time.timeScale = 1f; // Käynnistä peli
        crosshairObject.SetActive(true);
        ShowHiddenObjects();    // Palautetaan tähtäimet ym.
        PlayMusic(gameplayMusic, 1f);   // Vaihdetaan pelin musiikki
        screenTopPanel.SetActive(true);
    }

    public void Exit()                                  // Exit-napin toiminta
    {
        Debug.Log("Exit button pressed");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Toimii vain Unity Editorissa
#endif
    }
    
}
