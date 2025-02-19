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
    public float Sensitivity;
    public float Quality;
    public float RenderScale;
    public float SfxVol;
    public float MasterVol;
    public float MusicVol;
    public int Resolution;
}

public class Settings : MonoBehaviour
{
    public bool SaveOnUpdate = true;
    private bool PlayerAvailable;

    [Header("Sliders")]
    public Slider sensitivitySlider;
    public Slider renderScaleSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    [Header("Dropdown")]
    public TMP_Dropdown graphicsTierSettings;
    public TMP_Dropdown resolutionDropdown;

    [Header("CustomCheckButton")]

    private Camera cam;
    private PlayerLook look;

    [Header("Mixers")]
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup masterMixer;

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

        float targetAspectRatio = 16f / 9f;

        int targetWidth = 640;
        int targetHeight = 360;

        // Iterate through all resolutions
        foreach (Resolution resolution in resolutions)
        {
            if (resolution.width >= targetWidth && resolution.height >= targetHeight)
            {
                float aspectRatio = (float)resolution.width / (float)resolution.height;

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

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionStringList);
    }

    public void UpdateSettings()
    {
        if (PlayerAvailable)
        {
            look.Sensitivity = sensitivitySlider.value;
        }

        //Render & Quality
        cam.farClipPlane = renderScaleSlider.value;
        QualitySettings.SetQualityLevel(graphicsTierSettings.value);

        if (selectedResolutionList != null && selectedResolutionList.Count > 0 && resolutionDropdown.options.Count > 0)
        {
            int selectedResolutionIndex = Mathf.Clamp(resolutionDropdown.value, 0, selectedResolutionList.Count - 1);

            Resolution selectedResolution = selectedResolutionList[selectedResolutionIndex];

            Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.FullScreenWindow, selectedResolution.refreshRate);
        }

        //Sounds
        musicMixer.audioMixer.SetFloat("Volume", musicSlider.value);
        sfxMixer.audioMixer.SetFloat("Volume", sfxSlider.value);
        masterMixer.audioMixer.SetFloat("Volume", masterSlider.value);

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
            SfxVol = sfxSlider.value,
            MasterVol = masterSlider.value
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
        masterSlider.value = Save.MasterVol;


        UpdateSettings();
    }

    public void Exit()
    {
        Application.Quit();
    }
}