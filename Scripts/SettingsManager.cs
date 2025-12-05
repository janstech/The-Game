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





using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown sensitivityDropdown;
    public float mouseSensitivity = 1f;
    public Button muteButton;
    public Sprite volumeOnSprite;   
    public Sprite volumeOffSprite;               

    private bool isMuted = false;
    public Image buttonImage;

    void Start()
    {
        // Ladataan edellisen pelin asetus
        int savedIndex = PlayerPrefs.GetInt("SensitivityIndex", 1);
        sensitivityDropdown.value = savedIndex;
        ApplySensitivity(savedIndex);
        //buttonImage = GetComponent<Image>();
        UpdateButtonIcon();
        
    }

    public void OnSensitivityChanged(int index)
    {
        ApplySensitivity(index);
        PlayerPrefs.SetInt("SensitivityIndex", index);
    }

        public void ToggleMute()                // Mute-nappi
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        UpdateButtonIcon();
        Debug.Log("Mute: " + isMuted);
    }



        void UpdateButtonIcon()     // Mute-toiminnon sprite-imaget
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isMuted ? volumeOffSprite : volumeOnSprite;
        }
    }

    void ApplySensitivity(int index)        // Hiiren herkkyys alasvetovalikossa
    {
        switch (index)
        {
            case 0:
                mouseSensitivity = 550f;
                break;
            case 1:
                mouseSensitivity = 750f;
                break;
            case 2:
                mouseSensitivity = 950f;
                break;
        }

        // Jos käytetään esim. PlayerControlleria, välitä arvo sinne:
        /* PlayerController controller = FindObjectOfType<PlayerController>();
         if (controller != null)
         {
             controller.SetMouseSensitivity(mouseSensitivity);
         }*/

        SetMouseSensitivity(mouseSensitivity);                       // Välitetään arvo CameraManagerille

    }

        public void SetMouseSensitivity(float sensitivity)
    {
        CameraManager cm = FindAnyObjectByType<CameraManager>();
        if (cm != null)
        {
            cm.mouseSensitivity = sensitivity;
            Debug.Log("Mouse sensitivity asetettu arvoon: " + sensitivity);
        }
        else
        {
            Debug.LogWarning("CameraManager ei löytynyt!");
        }
    }
}
