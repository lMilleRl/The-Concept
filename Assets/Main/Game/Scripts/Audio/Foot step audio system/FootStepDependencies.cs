using UnityEngine;

public readonly struct FootStepDependencies
{
    public readonly Transform StepsSource;
    public readonly ISurfaceDetector SurfaceDetector;
    public readonly IStepEventSource StepEventSource;
    public readonly Vector2 FeetOffset;
    public readonly FootStepAudioData AudioData;
    public readonly AudioSource SoundsPlayer;

    public FootStepDependencies(
        Transform stepsSource,
        ISurfaceDetector surfaceDetector,
        IStepEventSource stepEventSource,
        Vector2 feetOffset,
        FootStepAudioData audioData,
        AudioSource soundsPlayer)
    {
        StepsSource = stepsSource;
        SurfaceDetector = surfaceDetector;
        StepEventSource = stepEventSource;
        FeetOffset = feetOffset;
        AudioData = audioData;
        SoundsPlayer = soundsPlayer;
    }
}
