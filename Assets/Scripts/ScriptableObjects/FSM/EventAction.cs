
public enum ActionType
{
    Sonar = 0,
    Default = -1
}

[System.Serializable]
public struct EventAction
{
    public Action action;
    public ActionType type;
}
