using System.Linq; // for All()
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/TransformDefaultToSun")]
public class TransformDefaultToSunDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;

        // Only true if current state matches and Sun powerup is present
        if (bat.currentPowerupTypes.Contains(PowerupType.Sun))
        {
            return true;
        }

        return false;
    }
}
