using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : Item
{
    private void Update()
    {
        if (!pv.IsMine || !active)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseAbility();
        }
    }

    public override void Enable()
    {
        active = true;
    }

    public override void Disable()
    {
        active = false;
        playerController.abilityImage.color = playerController.abilityCooldownColor;
        playerController.UpdateAbilityCooldown(0);
    }

    public abstract void UseAbility();

    public IEnumerator StartCooldown()
    {
        float timer = 0;
        
        while(timer < ((AbilityInfo)itemInfo).cooldown)
        {
            timer += Time.deltaTime;
            playerController.UpdateAbilityCooldown(timer / ((AbilityInfo)itemInfo).cooldown);
            yield return null;
        }
        
        Enable();
        playerController.abilityImage.color = Color.white;
    }
}
