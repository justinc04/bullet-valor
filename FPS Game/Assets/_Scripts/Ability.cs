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
            StartCoroutine(UseAbility());
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

    public abstract IEnumerator UseAbility();
}
