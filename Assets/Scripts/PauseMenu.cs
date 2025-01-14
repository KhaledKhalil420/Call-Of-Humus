using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;

    [Header("UI Elements")] // UI Elements
    public Slider sensitivtySlider;
    public Slider masterSlider;
    public Slider musicSlider;
    public TMP_Dropdown graphicsDropdown;
    public Transform parent;

    [Header("References")] // References
    public PlayerLook playerLook;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup musicGroup;
    public PlayerInventory playerInventory;

    private void Awake()
    {
        LoadValues();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        isPaused = !isPaused;
        if(isPaused)
            PauseGame();

        else
            ResumeGame();

        parent.gameObject.SetActive(isPaused);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Save Values On Change
    public void Save()
    {
        
    }
    
    //Load UI Values
    public void PauseGame()
    {
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadUiValues();

        playerLook.disableLook = true;
        playerInventory.canShoot = false;
    }

    //Resume
    public void ResumeGame()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerLook.disableLook = false;
        playerInventory.canShoot = true;

        LoadValues();
    }

    private void LoadUiValues()
    {
        sensitivtySlider.value = PlayerPrefs.GetFloat("sensitivty");
        masterSlider.value = PlayerPrefs.GetFloat("Master");
        musicSlider.value = PlayerPrefs.GetFloat("Music");   
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics");  
    }

    //Load References Values
    public void LoadValues()
    {
        playerLook.Sensitivity = sensitivtySlider.value;
        masterGroup.audioMixer.SetFloat("Volume", masterSlider.value);
        musicGroup.audioMixer.SetFloat("Volume", musicSlider.value);
        QualitySettings.SetQualityLevel(graphicsDropdown.value); 
    }
}
