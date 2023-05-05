using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    private bool gatherInput = true;

    private void Awake()
    {
        Instance = this;
    }

    public void GatherInput(bool state)
    {
        gatherInput = state;
    }

    public Vector2 GetAxis()
    {
        if (!gatherInput)
        {
            return Vector2.zero;
        }

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool GetWalkButton()
    {
        if (!gatherInput)
        {
            return false;
        }

        return Input.GetButton("Walk");
    }

    public bool GetJumpButtonDown()
    {
        if (!gatherInput)
        {
            return false;
        }

        return Input.GetButtonDown("Jump");
    }

    public bool GetJumpButtonUp()
    {
        if (!gatherInput)
        {
            return false;
        }

        return Input.GetButtonUp("Jump");
    }

    public bool GetReloadButton()
    {
        if (!gatherInput)
        {
            return false;
        }

        return Input.GetButtonDown("Reload");
    }

    public bool GetAbilityButton()
    {
        if (!gatherInput)
        {
            return false;
        }

        return Input.GetButtonDown("Ability");
    }
}
