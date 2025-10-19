
using UnityEngine;
using System;
[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public StateTransformMap[] map;

    public override bool Decide(StateController controller)
    {
        BatStateController m = (BatStateController)controller;
        // we assume that the state is named (string matched) after one of possible values in MarioState
        // convert between current state name into MarioState enum value using custom class EnumExtension
        // you are free to modify this to your own use
        BatState toCompareState = EnumExtension.ParseEnum<BatState>(m.currentState.name);

        // loop through state transform and see if it matches the current transformation we are looking for
        for (int i = 0; i < map.Length; i++)
        {
            bool stateMatch = toCompareState == map[i].fromState;
            bool powerupMatch = m.currentPowerupType == map[i].powerupCollected;
            Debug.Log($"[Decision:Transform] {name} | StateMatch={stateMatch} ({toCompareState} vs {map[i].fromState}) | PowerupMatch={powerupMatch} ({m.currentPowerupType} vs {map[i].powerupCollected})");

            if (stateMatch && powerupMatch)
            {
                return true;
            }
        }

        return false;

    }
}

[System.Serializable]
public struct StateTransformMap
{
    public BatState fromState;
    public PowerupType powerupCollected;
}
