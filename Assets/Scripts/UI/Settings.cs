using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class SaveSettings
{
    public float Sensitivity, Quality, RenderScale, SfxVol, MusicVol;
    public int Resolution;
}

public class Settings : MonoBehaviour
{
    public bool SaveOnUpdate = true;
    private bool PlayerAvailable;
    
    [Header("Sliders")]
    public Slider sensitivitySlider, renderScaleSlider, musicSlider, sfxSlider;

    [Header("Dropdown")]
    public TMP_Dropdown graphicsTierSettings, resolutionDropdown;

    [Header("CustomCheckButton")]

    private Camera cam;
    private PlayerLook look;

    [Header("Mixers")]
    public AudioMixerGroup musicMixer, sfxMixer;


    private Resolution[] resolutions;
    List<Resolution> selectedResolutionList = new List<Resolution>();

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        look = cam.GetComponentInParent<PlayerLook>();

        PlayerAvailable = GameObject.FindWithTag("Player") != null;
        
        AddResolutions();
        LoadPlayerData();
    }

void AddResolutions()
{
    resolutions = Screen.resolutions;
    List<string> resolutionStringList = new();

    // Define the target aspect ratio (16:9)
    float targetAspectRatio = 16f / 9f;

    // Define the target resolution width and height (adjust as needed)
    int targetWidth = 640;
    int targetHeight = 360;

    // Iterate through all resolutions
    foreach (Resolution resolution in resolutions)
    {
        // Check if the resolution width and height are greater than or equal to the target
        if (resolution.width >= targetWidth && resolution.height >= targetHeight)
        {
            // Calculate aspect ratio of the current resolution
            float aspectRatio = (float)resolution.width / (float)resolution.height;

            // Check if the aspect ratio is approximately equal to the target aspect ratio
            if (Mathf.Approximately(aspectRatio, targetAspectRatio))
            {
                string newResolution = resolution.width.ToString() + " x " + resolution.height.ToString();
                if (!resolutionStringList.Contains(newResolution))
                {
                    resolutionStringList.Add(newResolution);
                    selectedResolutionList.Add(resolution);
                }
            }
        }
    }

    // Clear existing options before adding new ones
    resolutionDropdown.ClearOptions();
    resolutionDropdown.AddOptions(resolutionStringList);
}


    public void UpdateSettings()
    {
        if (PlayerAvailable)
        {
            //Player options
            look.Sensitivity = sensitivitySlider.value;
        }

        //Render & Quality
        cam.farClipPlane = renderScaleSlider.value;
        QualitySettings.SetQualityLevel(graphicsTierSettings.value);
        // Ensure the resolution dropdown has options
        if (selectedResolutionList != null && selectedResolutionList.Count > 0 && resolutionDropdown.options.Count > 0)
        {
            // Get the selected index from the dropdown
            int selectedResolutionIndex = Mathf.Clamp(resolutionDropdown.value, 0, selectedResolutionList.Count - 1);

            // Get the selected resolution from the list
            Resolution selectedResolution = selectedResolutionList[selectedResolutionIndex];

            // Apply the selected resolution
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.FullScreenWindow, selectedResolution.refreshRate);
        }

        //Sounds
        musicMixer.audioMixer.SetFloat("Volume", musicSlider.value);
        sfxMixer.audioMixer.SetFloat("Volume", sfxSlider.value);


        if (SaveOnUpdate)
            SavePlayerData();

    }

    public void SavePlayerData()
    {
        SaveSettings Save = new()
        {
            Sensitivity = sensitivitySlider.value,

            //Render & Quality
            RenderScale = renderScaleSlider.value,
            Quality = graphicsTierSettings.value,
            Resolution = resolutionDropdown.value,

            //Sounds
            MusicVol = musicSlider.value,
            SfxVol = sfxSlider.value
        };

        string Json = JsonUtility.ToJson(Save);
        File.WriteAllText(Application.persistentDataPath + "/save.json", Json);
    }

    public void LoadPlayerData()
    {
        SaveSettings Save = JsonUtility.FromJson<SaveSettings>(File.ReadAllText(Application.persistentDataPath + "/save.json"));

        if (PlayerAvailable)
        {
            //Load player data
            sensitivitySlider.value = Save.Sensitivity;
        }

        //Load render & graphics data
        graphicsTierSettings.value = (int)Save.Quality;
        renderScaleSlider.value = Save.RenderScale;
        resolutionDropdown.value = Save.Resolution;

        //Load sound data
        musicSlider.value = Save.MusicVol;
        sfxSlider.value = Save.SfxVol;

        Debug.Log("Load");

        UpdateSettings();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
