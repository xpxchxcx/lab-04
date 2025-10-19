using UnityEngine;

public interface IPowerup
{
    void DestroyPowerup();
    void ApplyPowerup(MonoBehaviour i);

    PowerupType powerupType
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

    Default = -1,
    Flashlight = 0,
    Sun = 1,
}