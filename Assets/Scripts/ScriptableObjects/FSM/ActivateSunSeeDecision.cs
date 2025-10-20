using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/ActivateSunSee")]
public class ActivateSunSeeDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;

        if (bat.currentPowerupTypes.Contains(PowerupType.Sun) && bat.HasActivatedSun)
        {
            Debug.Log("SunSee to See");
            bat.HasActivatedSun = false;
            bat.currentPowerupTypes.Remove(PowerupType.Sun);
            return true; // triggers transition
        }

        return false;
    }
}
