using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageItems : PassiveItems
{
    protected override void AppliedModifire()
    {
        base.AppliedModifire();
        playerStats.CurrentMight += passiveItemsScriptableObject.Multiple;
    }
}
