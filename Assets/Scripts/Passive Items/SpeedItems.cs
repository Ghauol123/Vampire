using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedItems : PassiveItems
{
    protected override void AppliedModifire()
    {
        base.AppliedModifire();
        // playerStats.CurrentSpeed += passiveItemsScriptableObject.Multiple;
    }
}
