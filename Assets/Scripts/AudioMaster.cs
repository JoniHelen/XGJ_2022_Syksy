using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
public class AudioMaster : MonoBehaviour
{
    [SerializeField] List<AudioSource> sources = new();
    [SerializeField] List<Sound> sounds = new();
    [SerializeField] AudioMixerGroup audioMixer;

    [SerializeField] TextMeshProUGUI volumeText;
    [SerializeField] Slider volumeSlider;

    public float CurrentVolume;

    public static AudioMaster instance;

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 5; i++)
        {
            sources.Add(gameObject.AddComponent<AudioSource>());
            sources[i].outputAudioMixerGroup = audioMixer;
        }

        if (!PlayerPrefs.HasKey("Volume"))
        {
            CurrentVolume = 1;
            volumeSlider.value = 1;
        }
        else
        {
            CurrentVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = CurrentVolume;
        }
        UpdateVolume();
    }

    public void ChangeVolume()
    {
        audioMixer.audioMixer.SetFloat("MasterVolume", 30 * volumeSlider.value - 30);
        CurrentVolume = volumeSlider.value;
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        volumeText.text = (100 * CurrentVolume).ToString("Volume: ###");
    }

    public void StopClockSounds()
    {
        sources.Find(source => {
            if (source.clip != null)
            {
                return source.clip.name == "clock_ticking_timed";
            }
            else return false;
        }).Stop();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Volume", CurrentVolume);
    }

    public void PlaySound(string name, bool pitch = false, bool looping = false)
    {
        Sound sound = sounds.Find(clip => clip.name == name);
        AudioSource source = sources.Find(source => source.isPlaying == false);

        if (source != null)
        {
            source.volume = sound.volume;

            if (pitch)
                source.pitch = sound.pitch;
            else
                source.pitch = 1f;

            if (looping)
            {
                source.loop = true;
                source.clip = sound.clip;
                source.Play();
            }
            else
            {
                source.PlayOneShot(sound.clip);
            }
        }
    }
}
