using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;

[System.Serializable]
public struct Sounds
{
    public List<Sound> sounds;
}

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    [SerializeField] private Sounds s;
    public bool startFadeIn;

    public AudioMixerGroup DefaultAudioMixer;

    public bool destroyOnSceneLoad;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        DefaultAudioMixer.audioMixer.SetFloat("Volume", -80f);
        StartCoroutine(FadeIn());

        SetupAudioManager();
    }
    

    public void DestroyManager()
    {
        StartCoroutine(FadeOut(true));
    }

    void SetupAudioManager()
    {
        foreach (Sound soundToPlay in s.sounds)
        {
            // Create AudioSources for each sound
            soundToPlay.Source = gameObject.AddComponent<AudioSource>();

            // AudioSource Data
            soundToPlay.Source.pitch = soundToPlay.Pitch;
            soundToPlay.Source.clip = soundToPlay.Clip;
            soundToPlay.Source.loop = soundToPlay.Loop;
            soundToPlay.Source.volume = soundToPlay.Volume;
            soundToPlay.Source.outputAudioMixerGroup = soundToPlay.Mixer;

            if (soundToPlay.PlayOnAwake)
                soundToPlay.Source.Play();
        }
    }

    //-------------------Play sound

    public void PlaySound(string SoundName)
    {
        foreach (Sound soundToPlay in s.sounds)
            if (soundToPlay.SoundName == SoundName)
            {
                soundToPlay.Source.Stop();
                soundToPlay.Source.Play();
                break;
            }
    }

    public void PlaySound(string SoundName, float MinPitch, float MaxPitch)
    {
        foreach (Sound soundToPlay in s.sounds)
            if (soundToPlay.SoundName == SoundName)
            {
                soundToPlay.Source.Stop();
                soundToPlay.Source.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
                soundToPlay.Source.Play();
                break;
            }
    }

    public void StopSound(string SoundName)
    {
        foreach (Sound soundToPlay in s.sounds)
            if (soundToPlay.SoundName == SoundName)
            {
                soundToPlay.Source.Stop();
                return;
            }
    }

    public void ReplaceSoundClip(string SoundName, AudioClip NewClip)
    {
        foreach (Sound soundToPlay in s.sounds)
            if (soundToPlay.SoundName == SoundName)
            {
                soundToPlay.Source.clip = NewClip;
                return;
            }
    }

    //-------------------Effects

    public IEnumerator FadeIn()
    {
        DefaultAudioMixer.audioMixer.SetFloat("Volume", -80f);
        float currentTime = 0f;

        while (currentTime < 3)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(-80f, 0f, currentTime / 3);
            DefaultAudioMixer.audioMixer.SetFloat("Volume", newVolume);
            yield return null;
        }

        DefaultAudioMixer.audioMixer.SetFloat("Volume", 0f);
    }

    public IEnumerator FadeOut(bool destroyOnSceneLoad)
    {
        DefaultAudioMixer.audioMixer.SetFloat("Volume", 0);
        float currentTime = 0f;

        while (currentTime < 3)
        {
            currentTime += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(0, -90f, currentTime / 3);
            DefaultAudioMixer.audioMixer.SetFloat("Volume", newVolume);
            yield return null;
        }

        DefaultAudioMixer.audioMixer.SetFloat("Volume", -90f);

        if(destroyOnSceneLoad)
            Destroy(gameObject);
    }

    public IEnumerator FadeOutReplace(string audioName, AudioClip clip)
    {
        AudioSource source = new();
        foreach (Sound soundToPlay in s.sounds)
        {
            if (soundToPlay.SoundName == audioName)
            {
                source = soundToPlay.Source;
                break;
            }
        }


        float currentTime = 0f;
        while (currentTime < 3)
        {
            currentTime += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(source.volume, 0, currentTime / 3);
            yield return null;
        }

        source.clip = clip;

        float currentTime2 = 3f;
        while (currentTime2 < 6)
        {
            currentTime += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(source.volume, 1, currentTime / 3);
            yield return null;
        }
    }
}

[System.Serializable]
public class Sound
{
    [Space(10)]
    //----SoundData
    public string SoundName;
    public bool Loop;
    public bool PlayOnAwake;

    [Space(10)]
    //----Sliders
    [Range(0, 1)] public float Volume;
    [Range(0, 3)] public float Pitch = 1;

    [Space(10)]
    //----AudioSource Data
    [HideInInspector]
    public AudioSource Source;

    public AudioMixerGroup Mixer;
    public AudioClip Clip;
}