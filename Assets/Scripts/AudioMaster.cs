using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    [SerializeField] List<AudioSource> sources = new();
    [SerializeField] List<Sound> sounds = new();

    public static AudioMaster instance;

    private void Awake()
    {
        instance = this;
    }
    public void PlaySound(string name)
    {
        Sound clip = sounds.Find(clip => clip.name == name);
        AudioSource source = sources.Find(source => source.isPlaying == false);

        if (source != null)
        {
            source.volume = clip.volume;
            source.pitch = clip.pitch;
            source.PlayOneShot(clip.clip);
        }
    }
}
