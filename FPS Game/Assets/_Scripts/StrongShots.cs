using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongShots : Skill
{
    [SerializeField] float damageMultiplier;

    private void Start()
    {
        if (active)
        {
            playerController.damageMultiplier = damageMultiplier;
        }
    }
}
