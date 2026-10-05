using UnityEngine;

public class ClimbingAudioStrategy : IStepEffectStrategy
{
    private AudioSource _sourceClimbingSound;
    private AudioClip[] _climbingClips;
    private float _minInterval;
    private float _lastPlayTime = float.NegativeInfinity;

    public ClimbingAudioStrategy(AudioSource sourceClimbingSound, AudioClip[] climbingClips, float minInterval)
    {
        _sourceClimbingSound = sourceClimbingSound;
        _climbingClips = climbingClips;
        _minInterval = minInterval;
    }

    public void Execute(StepEffectContext context)
    {
        if (_climbingClips == null || _climbingClips.Length == 0) return;
        if (Time.time - _lastPlayTime < _minInterval) return;

        _lastPlayTime = Time.time;
        _sourceClimbingSound.PlayOneShot(_climbingClips[Random.Range(0, _climbingClips.Length)]);
    }
}
