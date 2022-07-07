using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongShots : Skill
{
    [SerializeField] float damageMultiplier;

    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GameManager.Instance.playerManager;
    }

    private void Start()
    {
        if (active)
        {
            playerManager.damageMultiplier = damageMultiplier;
        }
    }
}
