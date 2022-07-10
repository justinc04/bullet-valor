using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBullets : Skill
{
    [SerializeField] int moneyPerDamage;

    private void Start()
    {
        if (active)
        {
            playerController.damageMoney = moneyPerDamage;
        }
    }
}
