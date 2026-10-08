using UnityEngine;

public readonly struct CutsceneMoveToTargetCommand
{
    public Transform Target { get; }
    public float SpeedMultiplier { get; }
    public float StoppingDistance { get; }

    public CutsceneMoveToTargetCommand(Transform target, float speedMultiplier, float stoppingDistance)
    {
        Target = target;
        SpeedMultiplier = speedMultiplier;
        StoppingDistance = stoppingDistance;
    }
}
