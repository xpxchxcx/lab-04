using UnityEngine;

public interface IPowerup
{
    void DestroyPowerup();
    void ApplyPowerup(MonoBehaviour i);

    PowerupType powerupType
    {
        get;
    }

    bool hasSpawned
    {
        get;
    }
}


public interface IPowerupApplicable
{
    public void RequestPowerupEffect(IPowerup i);
}

public enum PowerupType
{
    Flashlight = 0,
    Sun = 1,
}