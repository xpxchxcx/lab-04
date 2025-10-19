using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SeeAll")]
public class SeeAllAction : Action
{
    public Light2D globalDarkness;
    public Color boostedColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
    public float brightDuration = 2f;
    public float fadeDuration = 1f;

    public override void Act(StateController controller)
    {
        BatStateController bat = (BatStateController)controller;
        bat.TriggerFlashlight(globalDarkness, boostedColor, brightDuration, fadeDuration);
    }
}
