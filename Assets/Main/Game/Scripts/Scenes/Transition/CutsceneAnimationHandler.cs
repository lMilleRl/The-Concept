using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneAnimationHandler : CutscenePlayer
{
    [SerializeField] private VideoPlayer _clipsPlayer;
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        _clipsPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _clipsPlayer.controlledAudioTrackCount = 1;
        _clipsPlayer.SetTargetAudioSource(0, _audioSource);
    }

    public override IEnumerator PlayCutscene(CutsceneData cutscene, Action onStarted, Action onFadeOut)
    {
        if (cutscene.Clip == null)
        {
            onFadeOut?.Invoke();
            yield break;
        }

        _clipsPlayer.clip = cutscene.Clip;
        _clipsPlayer.Play();
        yield return new WaitUntil(() => _clipsPlayer.isPlaying);
        onStarted?.Invoke();

        var fadeOutStartTime = Math.Max(0d, _clipsPlayer.length - cutscene.UIFadeOutDurationInSec);
        yield return new WaitUntil(() => !_clipsPlayer.isPlaying || _clipsPlayer.time >= fadeOutStartTime);

        onFadeOut?.Invoke();
        yield return new WaitUntil(() => !_clipsPlayer.isPlaying);
    }

    public override void StopCurrentCutscene()
    {
        _clipsPlayer.Stop();
    }
}
