using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SeeAll")]
public class SeeAllAction : Action
{
    public float boostedOuterRadius = 15f;
    public float normalOuterRadius = 1.5f;
    public float brightDuration = 2f;
    public float fadeDuration = 1f;

    public override void Act(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;
        bat.TriggerFlashlight(boostedOuterRadius, normalOuterRadius, brightDuration, fadeDuration);
    }
}
