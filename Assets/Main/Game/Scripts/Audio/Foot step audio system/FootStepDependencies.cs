using UnityEngine;

public readonly struct FootStepDependencies
{
    public readonly Transform StepsSource;
    public readonly ISurfaceDetector SurfaceDetector;
    public readonly IStepEventSource StepEventSource;
    public readonly Transform FeetPoint;
    public readonly FootStepAudioData AudioData;
    public readonly AudioSource SoundsPlayer;

    public FootStepDependencies(
        Transform stepsSource,
        ISurfaceDetector surfaceDetector,
        IStepEventSource stepEventSource,
        Transform feetPoint,
        FootStepAudioData audioData,
        AudioSource soundsPlayer)
    {
        StepsSource = stepsSource;
        SurfaceDetector = surfaceDetector;
        StepEventSource = stepEventSource;
        FeetPoint = feetPoint;
        AudioData = audioData;
        SoundsPlayer = soundsPlayer;
    }
}
