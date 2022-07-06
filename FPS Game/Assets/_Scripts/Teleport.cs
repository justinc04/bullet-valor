using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Teleport : Ability
{
    [SerializeField] float teleportSpeed;
    [SerializeField] GameObject teleportMarkerPrefab;
    [SerializeField] GameObject teleportEffectPrefab;
    private GameObject teleportMarker;
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private bool savedLocation;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerController = playerGameObject.GetComponent<PlayerController>();
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
    }

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
            pv.RPC("RPC_Teleport", RpcTarget.All);
            Disable();
            playerAudio.Play("Teleport");
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

    private void OnDestroy()
    {
        if (teleportMarker != null)
        {
            Destroy(teleportMarker);
        }
    }
}
