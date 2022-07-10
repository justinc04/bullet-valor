using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public GameObject playerGameObject;
    public PlayerAudio playerAudio;
    [HideInInspector] public bool active;
    [HideInInspector] public PhotonView pv;
    [HideInInspector] public PlayerManager playerManager;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerMovement playerMovement;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerManager = GameManager.Instance.playerManager;
        playerController = playerGameObject.GetComponent<PlayerController>();
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
    }

    public abstract void Enable();

    public abstract void Disable();
}
