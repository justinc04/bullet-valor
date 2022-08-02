using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Heal : Ability
{
    [SerializeField] float amount;

    public override void UseAbility()
    {
        if (playerController.currentHealth == playerController.maxHealth)
        {
            return;
        }

        UseHeal();
    }

    void UseHeal()
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
        playerController.currentHealth = Mathf.Min(playerController.currentHealth + amount, playerController.maxHealth);

        if (pv.IsMine)
        {
            playerController.UpdateHealth();
        }
    }
}
