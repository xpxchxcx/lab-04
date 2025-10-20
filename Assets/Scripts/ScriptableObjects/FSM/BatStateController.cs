using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BatStateController : StateController
{
    private Coroutine sunRayRoutine;
    public PowerupType currentPowerupType = PowerupType.Default;
    public BatState shouldBeNextState = BatState.DefaultBat;


    public bool HasActivatedSun { get; set; } = false;
    public override void Start()
    {
        base.Start();
        GameRestart(); // clear powerup in the beginning, go to start state
    }

    // this should be added to the GameRestart EventListener as callback
    public void GameRestart()
    {
        // clear powerup
        currentPowerupType = PowerupType.Default;
        // set the start state
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }

    public void SonarPulse()
    {
        this.currentState.DoEventTriggeredActions(this, ActionType.Sonar);
        HasActivatedSun = true;
    }


    public void TriggerSunRay(ParticleSystem psPrefab, float startRate, float boostedRate, float duration)
    {
        currentPowerupType = PowerupType.Sun;

        if (sunRayRoutine != null)
            StopCoroutine(sunRayRoutine);

        sunRayRoutine = StartCoroutine(HandleSunRay(psPrefab, startRate, boostedRate, duration));
    }


    private IEnumerator HandleSunRay(ParticleSystem psPrefab, float startRate, float boostedRate, float duration)
    {
        // Instantiate or reuse a particle system
        ParticleSystem ps = Instantiate(psPrefab, transform.position, Quaternion.identity, transform);
        var emission = ps.emission;

        // Boost emission rate
        var rate = emission.rateOverTime;
        rate.constant = boostedRate;
        emission.rateOverTime = rate;

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert emission rate
        rate.constant = startRate;
        emission.rateOverTime = rate;

        Destroy(ps.gameObject);
    }

    public void TriggerFlashlight(float boostedOuterRadius, float normalOuterRadius, float brightDuration, float fadeDuration)
    {
        // Find the Light2D in the Bat¡¯s child (the LightSource)
        Light2D lightSource = GetComponentInChildren<Light2D>();
        if (lightSource == null) return;

        // Stop any previous coroutine if already running
        StopAllCoroutines();
        StartCoroutine(HandleFlashlight(lightSource, boostedOuterRadius, normalOuterRadius, brightDuration, fadeDuration));
    }

    private IEnumerator HandleFlashlight(Light2D light, float boostedOuterRadius, float normalOuterRadius, float brightDuration, float fadeDuration)
    {
        // Instantly boost light radius
        float originalOuterRadius = light.pointLightOuterRadius;
        light.pointLightOuterRadius = boostedOuterRadius;

        // Stay bright for duration
        yield return new WaitForSeconds(brightDuration);

        // Fade back down
        float elapsed = 0f;
        float startRadius = boostedOuterRadius;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            light.pointLightOuterRadius = Mathf.Lerp(startRadius, normalOuterRadius, elapsed / fadeDuration);
            yield return null;
        }

        // Ensure final value is correct
        light.pointLightOuterRadius = normalOuterRadius;
    }

    void Update()
    {
        base.Update();
    }

}