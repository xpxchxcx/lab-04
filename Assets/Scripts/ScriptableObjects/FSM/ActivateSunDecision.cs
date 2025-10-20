using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/ActivateSun")]
public class ActivateSunDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;

        Debug.Log("Current power up is" + bat.currentPowerupType);

        if (bat.currentPowerupType == PowerupType.Sun && bat.HasActivatedSun)
        {
            Debug.Log("Sunray to Default");
            bat.HasActivatedSun = false;
            bat.currentPowerupType = PowerupType.Default;
            return true; // triggers transition
        }

        return false;
    }
}
