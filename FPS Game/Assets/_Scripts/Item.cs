using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public GameObject playerGameObject;
    [HideInInspector] public bool active;
    [HideInInspector] public PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public abstract void Enable();

    public abstract void Disable();
}
