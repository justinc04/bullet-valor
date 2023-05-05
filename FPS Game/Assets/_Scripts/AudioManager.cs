using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Sound[] sounds;

    private void Awake()
    {
        Instance = this;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.maxDistance = sound.maxDistance;
            sound.source.loop = sound.loop;
        }
    }

    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetInt("Volume") / 100f;
    }

    public void Play(string name)
    {
        GetSound(name).source.Play();
    }

    public void Stop(string name)
    {
        GetSound(name).source.Stop();
    }

    public bool CheckSound(string name)
    {
        return GetSound(name).source.isPlaying;
    }

    Sound GetSound(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
        }

        return sound;
    }
}
