using UnityEngine;

public class ClimbingAudioStrategy : IStepEffectStrategy
{
    private AudioSource _sourceClimbingSound;
    private AudioClip[] _climbingClips;

    public ClimbingAudioStrategy(AudioSource sourceClimbingSound, AudioClip[] climbingClips)
    {
        _sourceClimbingSound = sourceClimbingSound;
        _climbingClips = climbingClips;
    }
    
    public void Execute(StepEffectContext context)
    {
        _sourceClimbingSound.PlayOneShot(_climbingClips[Random.Range(0, _climbingClips.Length)]);
    }
}
