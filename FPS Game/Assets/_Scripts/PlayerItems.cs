using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerItems : MonoBehaviourPunCallbacks
{
    public Item[] itemReferences;
    public List<Item> items = new List<Item>();

    private int itemIndex;
    private int previousItemIndex = -1;

    private PhotonView pv;
    private PlayerController playerController;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            foreach (ItemInfo item in GameManager.Instance.items)
            {
                Item itemToAdd = Array.Find(itemReferences, i => i.itemInfo == item);

                if (item.itemType == ItemInfo.ItemType.Equipable)
                {
                    items.Add(itemToAdd);
                }
                else
                {
                    itemToAdd.Enable();
                }
            }
        }
        else
        {
            foreach (string item in (string[])pv.Owner.CustomProperties["items"])
            {
                Item itemToAdd = Array.Find(itemReferences, i => i.itemInfo.itemName == item);

                if (itemToAdd.itemInfo.itemType == ItemInfo.ItemType.Equipable)
                {
                    items.Add(itemToAdd);
                }
                else
                {
                    itemToAdd.Enable();
                }
            }
        }

        EquipItem(0);
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (itemIndex >= items.Count - 1)
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
                EquipItem(items.Count - 1);
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
            GameManager.Instance.playerProperties["itemIndex"] = itemIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(GameManager.Instance.playerProperties);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pv.IsMine && targetPlayer == pv.Owner && items.Count > 0)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
}
