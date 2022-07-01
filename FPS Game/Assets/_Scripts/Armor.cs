using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Armor : Item
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = playerGameObject.GetComponent<PlayerController>();
        pv = GetComponent<PhotonView>();
    }

    public override void Enable() 
    {
        if (!pv.IsMine)
        {
            itemGameObject.SetActive(true);
        }

        playerController.currentHealth += ((ArmorInfo)itemInfo).armorHealth;
        playerController.UpdateHealth();
    }

    public override void Disable() { }
}
