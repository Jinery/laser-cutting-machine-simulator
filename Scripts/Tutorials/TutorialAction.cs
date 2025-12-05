using UnityEngine;

[System.Serializable]
public class TutorialAction
{
    public enum ActionType
    {
        PressButton,
        PlaceMaterial,
        StartCutting,
        OpenDoor,
        CloseDoor,
        PowerOn,
        PowerOff
    }

    public ActionType actionType;
    public string targetObjectName;
    public bool isCompleted = false;
}
