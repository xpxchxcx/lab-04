using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ClearPowerup")]
public class ClearPowerupAction : Action
{
    public override void Act(StateController controller)
    {
        BatStateController b = (BatStateController)controller;
        b.currentPowerupType = PowerupType.Default;
        Debug.Log("Powerup cleared, current powerup: " + b.currentPowerupType);
    }
}