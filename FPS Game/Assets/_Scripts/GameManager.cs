using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Hashtable playerProperties = new Hashtable();
    public PlayerManager playerManager;

    [SerializeField] GameObject killGraphic;

    public int score;
    public List<ItemInfo> items;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerProperties["score"] = score;
    }

    public void OnSpawn()
    {
        killGraphic.SetActive(false);
        AudioManager.Instance.audioListener.enabled = false;
    }

    public void OnDeath()
    {
        AudioManager.Instance.audioListener.enabled = true;
    }

    public void OnKill()
    {
        score++;
        playerProperties["score"] = score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        killGraphic.SetActive(true);
        AudioManager.Instance.Play("Kill");
    }
}
