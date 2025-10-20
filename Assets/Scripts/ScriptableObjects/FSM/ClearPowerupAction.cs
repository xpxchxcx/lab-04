using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ClearPowerup")]
public class ClearPowerupAction : Action
{
    public override void Act(StateController controller)
    {
        BatStateController b = (BatStateController)controller;
        b.currentPowerupTypes.Clear();
    }
}