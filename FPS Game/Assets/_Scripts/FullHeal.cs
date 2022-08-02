using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FullHeal : Ability
{
    public override void UseAbility()
    {
        if (playerController.currentHealth == playerController.maxHealth)
        {
            return;
        }

        UseFullHeal();
    }

    void UseFullHeal()
    {
        playerAudio.Play("Heal");
        Disable();
        pv.RPC("RPC_Heal", RpcTarget.Others);
        StartCoroutine(StartCooldown());
    }

    [PunRPC]
    void RPC_Heal()
    {
        pv.RPC("RPC_AddHealth", RpcTarget.All);
    }

    [PunRPC]
    void RPC_AddHealth()
    {
        playerController.currentHealth = playerController.maxHealth;

        if (pv.IsMine)
        {
            playerController.UpdateHealth();
        }
    }
}
