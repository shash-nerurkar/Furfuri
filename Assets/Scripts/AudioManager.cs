using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach(Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if(sound == null)
        {
            string allSoundNames = "";
            foreach(Sound s in sounds)
            {
                allSoundNames += s.name + ", ";
            }
            Debug.LogWarning("SOUND " + name + " NOT FOUND, AMONGST " + allSoundNames);
            return;
        }

        sound.audioSource.Play();
    }
}
