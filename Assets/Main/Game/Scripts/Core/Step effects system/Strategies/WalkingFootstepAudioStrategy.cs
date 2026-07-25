using UnityEngine;

public class WalkingFootstepAudioStrategy : IStepEffectStrategy
{
    private readonly FootStepAudioData _audioData;
    private readonly AudioSource _soundsPlayer;

    public WalkingFootstepAudioStrategy(FootStepAudioData audioData, AudioSource soundsPlayer)
    {
        _audioData = audioData;
        _soundsPlayer = soundsPlayer;
    }

    public void Execute(StepEffectContext context)
    {
        var sounds = _audioData.GetFootStepAudio(context.SurfaceType);
        if (sounds != null)
        {
            var randomSoundInd = Random.Range(0, sounds.Length);
            _soundsPlayer.PlayOneShot(sounds[randomSoundInd]);
        }
    }
}