using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPowerup : Action
{
    public enum Powerup { Jump, DoubleJump, Dash };

    public Powerup powerup;

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}
