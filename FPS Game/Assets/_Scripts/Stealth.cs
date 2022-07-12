using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : Skill
{
    [SerializeField] float speedMultiplier;
    [SerializeField] float accelerationMultiplier;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float soundMultiplier;
    [SerializeField] float landingMultiplier;
    [SerializeField] string[] sounds;

    private void Start()
    {
        if (active)
        {
            playerMovement.runSpeed *= speedMultiplier;
            playerMovement.walkSpeed *= speedMultiplier;
            playerMovement.groundAcceleration *= accelerationMultiplier;
            playerMovement.airAcceleration *= accelerationMultiplier;
            playerMovement.jumpHeight *= jumpMultiplier;
            playerMovement.landingDuration *= landingMultiplier;

            foreach (string sound in sounds)
            {
                playerAudio.GetSound(sound).source.volume *= soundMultiplier;
                playerAudio.GetSound(sound).source.maxDistance *= soundMultiplier;
            }
        }
    }
}
