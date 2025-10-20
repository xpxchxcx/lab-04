using System.Linq; // for All()
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/TransformDefaultToSee")]
public class TransformDefaultToSeeDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;

        // Only true if current state matches and Flashlight powerup is present
        if (bat.currentPowerupTypes.Contains(PowerupType.Flashlight))
        {
            return true;
        }

        return false;
    }
}
