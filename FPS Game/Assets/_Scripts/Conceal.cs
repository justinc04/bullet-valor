using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Conceal : Ability
{
    [SerializeField] float duration;
    [SerializeField] float shootDelay;

    public override void UseAbility()
    {
        StartCoroutine(UseConceal());
    }

    IEnumerator UseConceal()
    {
        pv.RPC("RPC_Conceal", RpcTarget.Others, false);
        playerController.weaponModels.transform.localScale = Vector3.zero;
        playerController.canShoot = false;
        playerAudio.Play("Conceal");
        Disable();
        yield return new WaitForSeconds(duration);
        pv.RPC("RPC_Conceal", RpcTarget.Others, true);
        playerController.weaponModels.transform.localScale = Vector3.one;
        StartCoroutine(DelayShooting());
        StartCoroutine(StartCooldown());
    }

    IEnumerator DelayShooting()
    {
        yield return new WaitForSeconds(shootDelay);
        playerController.canShoot = true;
    }

    [PunRPC]
    void RPC_Conceal(bool visible)
    {
        playerController.playerModel.gameObject.transform.localScale = visible ? Vector3.one : Vector3.zero;
        playerController.weaponModels.gameObject.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

}
