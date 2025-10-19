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

    public void TriggerFlashlight(Light2D globalDarkness, Color boostedColor, float brightDuration, float fadeDuration)
    {
        if (globalDarkness == null) return;
        StartCoroutine(HandleFlashlight(globalDarkness, boostedColor, brightDuration, fadeDuration));
    }

    private IEnumerator HandleFlashlight(Light2D light, Color boostedColor, float brightDuration, float fadeDuration)
    {
        Color originalColor = light.color;

        // Boost light immediately
        light.color = boostedColor;

        // Wait for bright duration
        yield return new WaitForSeconds(brightDuration);

        // Fade back to original
        Color startColor = light.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            light.color = Color.Lerp(startColor, originalColor, elapsed / fadeDuration);
            yield return null;
        }

        light.color = originalColor;
    }

    void Update()
    {
        Debug.Log($"[BAT] Powerup={currentPowerupType} | State={(currentState ? currentState.name : "None")}");
        base.Update();
    }

}