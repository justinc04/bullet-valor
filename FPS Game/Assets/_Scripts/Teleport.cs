using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Teleport : Ability
{
    [SerializeField] float timeBeforeShoot;
    [SerializeField] GameObject teleportMarkerPrefab;
    [SerializeField] GameObject teleportEffectPrefab;
    private GameObject teleportMarker;
    private bool savedLocation;

    public override void UseAbility()
    {
        if (!savedLocation)
        {
            if (!playerMovement.isGrounded)
            {
                return;
            }

            pv.RPC("RPC_SpawnMarker", RpcTarget.All, playerController.transform.position);
            savedLocation = true;
            playerAudio.Play("Teleport Marker");
        }
        else
        {
            StartCoroutine(DelayShooting());
            pv.RPC("RPC_Teleport", RpcTarget.All);
            Disable();
            savedLocation = false;
            playerAudio.Play("Teleport");
            StartCoroutine(StartCooldown());
        }
    }

    [PunRPC]
    void RPC_SpawnMarker(Vector3 position)
    {
        teleportMarker = Instantiate(teleportMarkerPrefab, position, Quaternion.identity);
    }

    [PunRPC]
    void RPC_Teleport()
    {
        GameObject effect = Instantiate(teleportEffectPrefab, playerMovement.groundCheck.position, Quaternion.identity);
        playerController.transform.position = teleportMarker.transform.position;
        Physics.SyncTransforms();
        Destroy(teleportMarker);
        Destroy(effect, 2);
    }

    IEnumerator DelayShooting()
    {
        playerController.canShoot = false;
        yield return new WaitForSeconds(timeBeforeShoot);
        playerController.canShoot = true;
    }

    private void OnDestroy()
    {
        if (teleportMarker != null)
        {
            Destroy(teleportMarker);
        }
    }
}
