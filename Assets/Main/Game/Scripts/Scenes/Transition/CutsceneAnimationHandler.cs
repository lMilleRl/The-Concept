using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneAnimationHandler : CutscenePlayer
{
    [SerializeField] private VideoPlayer _clipsPlayer;
    [SerializeField] private AudioSource _audioSource;

    private bool _isCutscenePlaying;
    private bool _isPausedByFocusLoss;
    private bool _isWaitingForFocusResume;
    private bool _hasPlaybackStarted;

    private void Awake()
    {
        _clipsPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
        _clipsPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _clipsPlayer.controlledAudioTrackCount = 1;
        _clipsPlayer.SetTargetAudioSource(0, _audioSource);
    }

    private void Update()
    {
        if (!_isWaitingForFocusResume || !_clipsPlayer.isPlaying)
            return;

        _isWaitingForFocusResume = false;
        _isPausedByFocusLoss = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!_isCutscenePlaying)
            return;

        if (!hasFocus)
        {
            if (_hasPlaybackStarted && !_clipsPlayer.isPlaying &&
                _clipsPlayer.time >= _clipsPlayer.length)
                return;

            _isPausedByFocusLoss = true;
            _isWaitingForFocusResume = false;
            _clipsPlayer.Pause();
            return;
        }

        if (!_isPausedByFocusLoss)
            return;

        _isWaitingForFocusResume = true;
        _clipsPlayer.Play();
    }

    public override IEnumerator PlayCutscene(CutsceneData cutscene, Action onStarted, Action onFadeOut)
    {
        if (cutscene.Clip == null)
        {
            onFadeOut?.Invoke();
            yield break;
        }

        _clipsPlayer.clip = cutscene.Clip;
        _isCutscenePlaying = true;
        _hasPlaybackStarted = false;
        _clipsPlayer.Play();
        yield return new WaitUntil(() => _clipsPlayer.isPlaying || !_isCutscenePlaying);
        if (!_isCutscenePlaying)
            yield break;

        _hasPlaybackStarted = true;
        onStarted?.Invoke();

        var fadeOutStartTime = Math.Max(0d, _clipsPlayer.length - cutscene.UIFadeOutDurationInSec);
        yield return new WaitUntil(() => !_isPausedByFocusLoss &&
            (!_clipsPlayer.isPlaying || _clipsPlayer.time >= fadeOutStartTime));

        onFadeOut?.Invoke();
        yield return new WaitUntil(() => !_isPausedByFocusLoss && !_clipsPlayer.isPlaying);

        _isCutscenePlaying = false;
        _hasPlaybackStarted = false;
        _isWaitingForFocusResume = false;
    }

    public override void StopCurrentCutscene()
    {
        _isCutscenePlaying = false;
        _hasPlaybackStarted = false;
        _isPausedByFocusLoss = false;
        _isWaitingForFocusResume = false;
        _clipsPlayer.Stop();
    }
}
