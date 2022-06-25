using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    private PhotonView pv;
    public GameObject controller;

    public Hashtable playerProperties = new Hashtable();
    public int score;
    
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            CreateController();
            playerProperties["score"] = score;
        }
    }

    void CreateController()
    {
        GameManager.Instance.killGraphic.SetActive(false);
        AudioManager.Instance.audioListener.enabled = false;
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint(PhotonNetwork.LocalPlayer.IsMasterClient);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
    }

    void DestroyController()
    {
        if (controller == null)
        {
            return;
        }

        PhotonNetwork.Destroy(controller);
    }

    public void Die()
    {
        DestroyController();
        AudioManager.Instance.audioListener.enabled = true;

        if (GameManager.Instance.killGraphic.activeSelf)
        {
            return;
        }

        Invoke("CreateController", 3);
    }

    public void Kill()
    {
        score++;
        playerProperties["score"] = score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        GameManager.Instance.killGraphic.SetActive(true);
        AudioManager.Instance.Play("Kill");

        Invoke("DestroyController", 3);
        Invoke("CreateController", 3);
    }
}
