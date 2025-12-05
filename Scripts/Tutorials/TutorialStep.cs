using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[System.Serializable]
public class TutorialStep
{
    public string stepName;
    [TextArea(3, 5)]
    public string instructionText;
    public VideoClip instructionVideo;
    public UnityEvent onStepStart, onStepComplete;
    public TutorialAction requiredAction;
}
