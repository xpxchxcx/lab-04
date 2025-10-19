using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SunRay")]
public class SunRayAction : Action
{
    public ParticleSystem particleSystemPrefab;
    public float startRate = 20f;
    public float boostedRate = 50f;
    public float duration = 10f;

    public override void Act(StateController controller)
    {
        BatStateController b = (BatStateController)controller;
        b.TriggerSunRay(particleSystemPrefab, startRate, boostedRate, duration);
    }
}
