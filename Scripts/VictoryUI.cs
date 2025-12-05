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
using UnityEngine.UI;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public GameObject panel;                     // Victory-paneeli
    public Button restartButton;
    public Button exitButton;
    public GameObject makerText;
    public TextMeshProUGUI victoryText;          // Teksti joka animoidaan
    public AudioClip victorySound;               // Loppumusiikki
    public float fadeDuration = 2f;

    [Header("Pulssi-asetukset")]
    public bool pulseText = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    private AudioSource audioSource;
    private Vector3 originalScale;

    void Start()
    {
        makerText.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        panel.SetActive(false); // Piilossa aluksi
    }

    public void ShowVictory()
    {
        panel.SetActive(true);
        originalScale = victoryText.transform.localScale; // Tallenna alkuperäinen koko
        StartCoroutine(FadeInText());

        if (victorySound != null)
        {
        audioSource.clip = victorySound;
        audioSource.Play();
        }


        AudioSource cameraAudio = Camera.main.GetComponent<AudioSource>(); // Pysäytetään taustamusiikki
        if (cameraAudio != null)
        cameraAudio.Stop();


        makerText.SetActive(true);

        Debug.Log("VictoryUI: Näytetään makerText ja napit");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    void Update()
    {
        if (pulseText && panel.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
            victoryText.transform.localScale = originalScale * scale;
        }
    }

    IEnumerator FadeInText()
    {
        float t = 0f;
        Color c = victoryText.color;
        c.a = 0f;
        victoryText.color = c;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            victoryText.color = c;
            yield return null;
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("ExitGame kutsuttu");
    }

}
