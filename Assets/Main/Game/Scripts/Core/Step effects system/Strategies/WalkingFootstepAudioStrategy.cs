using UnityEngine;

public class WalkingFootstepAudioStrategy : IStepEffectStrategy
{
    private readonly FootStepAudioData _audioData;
    private readonly AudioSource _soundsPlayer;

    private SurfaceType _lastSurfaceType = SurfaceType.None;
    private int _lastClipIndex = -1;

    public WalkingFootstepAudioStrategy(FootStepAudioData audioData, AudioSource soundsPlayer)
    {
        _audioData = audioData;
        _soundsPlayer = soundsPlayer;
    }

    public void Execute(StepEffectContext context)
    {
        SurfaceType surfaceType = context.SurfaceType;

        if (!_audioData.TryGetAudioStepData(surfaceType, out var data))
        {
            _lastSurfaceType = SurfaceType.None;
            return;
        }

        if (surfaceType != _lastSurfaceType)
            _lastClipIndex = -1;

        var clips = data.StepsAudio;
        if (clips == null || clips.Length == 0)
            return;

        int index = GetNextClipIndex(clips.Length);

        float minPitch = data.MinPitch == 0f ? 1f : data.MinPitch;
        float maxPitch = data.MaxPitch == 0f ? 1f : data.MaxPitch;

        if (minPitch > maxPitch)
        {
            (minPitch, maxPitch) = (maxPitch, minPitch);
        }

        _soundsPlayer.pitch = Random.Range(minPitch, maxPitch);
        _soundsPlayer.PlayOneShot(clips[index]);

        _lastClipIndex = index;
        _lastSurfaceType = surfaceType;
    }

    private int GetNextClipIndex(int clipsCount)
    {
        if (clipsCount == 1)
            return 0;

        if (_lastClipIndex < 0 || _lastClipIndex >= clipsCount)
            return Random.Range(0, clipsCount);

        int index = Random.Range(0, clipsCount);

        if (index == _lastClipIndex)
            index = _lastClipIndex++ % clipsCount;

        return index;
    }
}