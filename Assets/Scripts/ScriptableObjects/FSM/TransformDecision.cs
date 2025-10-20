using System.Linq; // for All()
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public StateTransformMap[] map;

    public override bool Decide(StateController controller)
    {
        BatStateController m = (BatStateController)controller;
        BatState toCompareState = EnumExtension.ParseEnum<BatState>(m.currentState.name);

        for (int i = 0; i < map.Length; i++)
        {
            bool stateMatch = toCompareState == map[i].fromState;

            // allPowerupsMatch is true only if requiredPowerups is non-empty
            // and all of them are present in currentPowerupTypes
            bool allPowerupsMatch = map[i].requiredPowerups.Length > 0 &&
                                    map[i].requiredPowerups.All(p => m.currentPowerupTypes.Contains(p));

            if (stateMatch && allPowerupsMatch)
            {
                return true;
            }
        }

        return false; // no match
    }

    [System.Serializable]
    public struct StateTransformMap
    {
        public BatState fromState;
        public PowerupType[] requiredPowerups;
    }
}
