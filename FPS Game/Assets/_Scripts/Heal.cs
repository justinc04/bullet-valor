using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Heal : Ability
{
    [SerializeField] float amount;
    [SerializeField] float shootDelay;

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
        StartCoroutine(DelayShooting());
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

    IEnumerator DelayShooting()
    {
        playerController.canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        playerController.canShoot = true;
    }
}
