using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Armor : Item
{
    public override void Enable() 
    {
        if (!pv.IsMine)
        {
            itemGameObject.SetActive(true);
        }

        playerController.maxHealth = playerController.currentHealth += ((ArmorInfo)itemInfo).armorHealth;
        playerController.UpdateHealth();
    }

    public override void Disable() { }
}
