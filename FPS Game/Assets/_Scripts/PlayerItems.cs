using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerItems : MonoBehaviourPunCallbacks
{
    [SerializeField] Item[] items;

    private int itemIndex;
    private int previousItemIndex = -1;

    private PhotonView pv;
    private PlayerController playerController;
    private PlayerManager playerManager;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerController = GetComponent<PlayerController>();
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            EquipItem(0);
        }
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        playerController.UpdateAmmo(((Gun)items[itemIndex]).ammo);
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        items[itemIndex].Enable();
        items[itemIndex].active = true;

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
            items[previousItemIndex].Disable();
            items[previousItemIndex].active = false;
        }

        previousItemIndex = itemIndex;

        if (pv.IsMine)
        {
            playerManager.playerProperties["itemIndex"] = itemIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerManager.playerProperties);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pv.IsMine && targetPlayer == pv.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
}
