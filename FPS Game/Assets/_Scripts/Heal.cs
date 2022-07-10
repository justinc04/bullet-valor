using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Heal : Ability
{
    [SerializeField] float amount;
    [SerializeField] float timeToHeal;

    public override void UseAbility()
    {
        if (playerController.currentHealth == playerController.maxHealth)
        {
            return;
        }

        StartCoroutine(UseHeal());
    }

    IEnumerator UseHeal()
    {
        playerAudio.Play("Heal");
        playerManager.canShoot = false;
        Disable();
        yield return new WaitForSeconds(timeToHeal);
        pv.RPC("RPC_Heal", RpcTarget.Others);
        playerManager.canShoot = true;
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
