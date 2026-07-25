using UnityEngine;

public readonly struct FootStepDependencies
{
    public readonly Transform StepsSource;
    public readonly ISurfaceDetector SurfaceDetector;
    public readonly FootStepAudioData AudioData;
    public readonly AudioSource SoundsPlayer;

    public FootStepDependencies(
        Transform stepsSource,
        ISurfaceDetector surfaceDetector,
        FootStepAudioData audioData,
        AudioSource soundsPlayer)
    {
        StepsSource = stepsSource;
        SurfaceDetector = surfaceDetector;
        AudioData = audioData;
        SoundsPlayer = soundsPlayer;
    }
}
