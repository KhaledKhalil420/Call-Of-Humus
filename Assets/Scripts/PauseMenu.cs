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
        PlayerPrefs.SetFloat("sensitivty", sensitivtySlider.value);
        PlayerPrefs.SetFloat("master", masterSlider.value);
        PlayerPrefs.SetFloat("music", musicSlider.value);
        PlayerPrefs.SetInt("Graphics", graphicsDropdown.value);
    }
    
    //Load UI Values
    public void PauseGame()
    {
        Time.timeScale = 0;

        sensitivtySlider.value = PlayerPrefs.GetFloat("sensitivty");
        masterSlider.value = PlayerPrefs.GetFloat("master");
        musicSlider.value = PlayerPrefs.GetFloat("music");   
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", graphicsDropdown.value);  

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

    //Load References Values
    public void LoadValues()
    {
        playerLook.Sensitivity = PlayerPrefs.GetFloat("sensitivty");
        masterGroup.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("master"));
        musicGroup.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("music"));
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics", graphicsDropdown.value)); 
    }
}
