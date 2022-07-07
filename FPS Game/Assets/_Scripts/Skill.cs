using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Item
{
    public override void Enable()
    {
        active = true;
    }

    public override void Disable()
    {
        active = false;
    }
}
