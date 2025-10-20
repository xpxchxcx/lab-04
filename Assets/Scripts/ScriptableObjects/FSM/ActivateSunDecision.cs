using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/ActivateSun")]
public class ActivateSunDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;

        // The powerup must have been collected
        if (bat.currentPowerupType == PowerupType.Sun)
        {
            Debug.Log("Checking ActivateSunDecision...");

            // We check a one-time flag toggled by SonarPulse() when player presses Space
            bool shouldActivate = bat.HasActivatedSun;
            if (shouldActivate)
            {
                // Reset flag so it only triggers once
                bat.HasActivatedSun = false;
                return true; // triggers transition
            }
        }

        return false;
    }
}
