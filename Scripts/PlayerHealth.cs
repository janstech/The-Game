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





using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // Käytetään kun halutaan ladata peli uudelleen tai siirtyminen Game Over-tilaan/sceneen
using UnityEngine.UI;      // Otetaan Unityn UI-kirjasto/komponentit käyttöön
using TMPro;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool hasPlayedWarning = false;   // Estetään varoitusäänen jatkuva toisto
    public TextMeshProUGUI healthText; 
    public GameObject gameOverText;

    public Slider healthSlider;
    public AudioClip damageSound;
    private AudioSource audioSource;
    public AudioClip warningSound;  // Ääni kun HP menee matalaksi
    public DamageFlash screenFlash;
	public Animator animator;
    public WeaponController weaponController;   // Estetään ampuminen kuollessa

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        UpdateHealthUI();
        gameOverText.SetActive(false);  // Ei näytetä Game Overia heti
        audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);   // Varmistus ettei mene alle 0
        healthSlider.value = currentHealth;
        UpdateHealthUI();

        Debug.Log("Player health: " + currentHealth);

        // Näytön punainen välähdys kun otetaan osumaa
        if (screenFlash != null)
        {
            screenFlash.TriggerFlash();
        }

        if (damageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

        if (!hasPlayedWarning && currentHealth <= 2 && warningSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(warningSound);
            hasPlayedWarning = true;
        }

        if (currentHealth <= 0)
            {
                GameOver();
            } 
    }

    void OnCollisionEnter(Collision collision)
    {
        // Jos törmätään Enemy-tagilla olevaan -> otetaan 1 damage
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    void UpdateHealthUI()  
    {
         healthText.text = "♥ " + currentHealth + "/" + maxHealth; // Health myös tekstinä/kuvana
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Parannettiin! Nykyinen HP: " + currentHealth);

        healthSlider.value = currentHealth;
        UpdateHealthUI();  // <- näyttää tekstissä ♥ HP määrän

        if (currentHealth > 2)      // Nollataan healthin varoitusääni, että se kuuluu taas kun elämät on kerätty yli kahteen
        {
            hasPlayedWarning = false;
        }
    }

    void GameOver()
    {
        weaponController.canShoot = false;
        gameOverText.SetActive(true);
        StartCoroutine(DelayedGameOver());   // Animaatio ja pieni viive
        GameManager.Instance.GameOver();     // Ilmoitetaan GameManagerille
    }

    IEnumerator DelayedGameOver()
    {
        animator.SetTrigger("IsDead");
        Debug.Log("Death animation triggered!");
        yield return new WaitForSeconds(3.0f); // Odotusaika sekuntia
        GameManager.Instance.GameOver();
        Time.timeScale = 0f;  // Pysäytetään peli
    }
}
