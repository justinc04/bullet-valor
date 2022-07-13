using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.maxDistance = sound.maxDistance;
            sound.source.loop = sound.loop;

            sound.source.spatialBlend = 1;
            sound.source.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    public void Play(string name)
    {
        pv.RPC("RPC_Play", RpcTarget.All, name);
    }

    [PunRPC]
    public void RPC_Play(string name)
    {
        GetSound(name).source.Play();
    }

    public void Stop(string name)
    {
        pv.RPC("RPC_Stop", RpcTarget.All, name);
    }

    [PunRPC]
    public void RPC_Stop(string name)
    {
        GetSound(name).source.Stop();
    }

    public bool CheckSound(string name)
    {
        return GetSound(name).source.isPlaying;
    }

    public Sound GetSound(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found.");
        }

        return sound;
    }
}
