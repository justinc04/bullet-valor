using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Dash : Item
{
    [SerializeField] float duration;
    [SerializeField] float speed;
    [SerializeField] ParticleSystem dashLines;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!pv.IsMine || !active)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(UseDash());
        }
    }

    public override void Enable()
    {
        active = true;
    }

    public override void Disable()
    {
        active = false;
    }

    IEnumerator UseDash()
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
        yield return new WaitForSeconds(duration);
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
