using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Dash : Ability
{
    [SerializeField] float speed;
    [SerializeField] ParticleSystem dashLines;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
    }

    public override IEnumerator UseAbility()
    {
        ControlMovement(true);
        playerMovement.speed = speed;

        if (playerMovement.direction == Vector3.zero)
        {
            playerMovement.direction = transform.forward;
        }

        playerMovement.velocity = Vector3.zero;
        dashLines.Play();
        playerAudio.Play("Dash");
        Disable();
        yield return new WaitForSeconds(((AbilityInfo)itemInfo).duration);
        ControlMovement(false);
        dashLines.Stop();
    }

    void ControlMovement(bool control)
    {
        playerMovement.speedControlled = control;
        playerMovement.directionControlled = control;
        playerMovement.gravityControlled = control;
        playerMovement.jumpControlled = control;
    }
}
