using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour, ITransitionHandler
{
    public static ITransitionHandler Instance;

    [SerializeField] private GameObject _root;
    [SerializeField] private Image _fadePanel;
    [SerializeField] private CanvasGroup _fadeCutsceneGroup;
    [SerializeField] private CutscenePlayer _cutscenesHandler;
    [SerializeField] private bool _isTimeStopOnTransition = true;

    private Coroutine _currentTransition;
    private Coroutine _currentCutscenePlaying;
    private Coroutine _currentClipPlaying;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(_root);
            return;
        }

        Instance = this;
    }

    public void StartTransition(TransitionData transitionData)
    {
        _cutscenesHandler.StopCurrentCutscene();
        StopAllCoroutines();
        _currentTransition = StartCoroutine(Translate(transitionData));
    }

    private void SwitchToPassiveGameState()
    {
        switch (GameStateManager.Instance.CurrentState)
        {
            case GameState.PassiveShow:
                return;
            case GameState.MovementCutscene:
                return;
            default:
                GameStateManager.Instance.SetState(GameState.PassiveShow);
                return;
        }
    }

    private IEnumerator Translate(TransitionData transitionData)
    {
        SwitchToPassiveGameState();
        VolumeAudioManager.Instance.ResumeCutscene();

        FadeOutSound(transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);
        yield return FadeInTransitionPanel
            (transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);

        // Пауза после затухания экрана: даём доиграть звукам триггера (дверь, шаги) до уничтожения сцены
        yield return new WaitForSecondsRealtime(transitionData.PauseBeforeLoadInSec);

        if (!string.IsNullOrEmpty(transitionData.SceneName))
        {
            VolumeAudioManager.Instance.MuteCutscene();
            SceneManager.LoadScene(transitionData.SceneName);
            GameStateManager.Instance.SetState(GameState.PassiveShow);
            if (_isTimeStopOnTransition)
                Time.timeScale = 0f;
            VolumeAudioManager.Instance.MuteGameplay();
        }

        yield return PlayCutscenes(transitionData.OwnCutscenesData);

        if (_isTimeStopOnTransition)
            Time.timeScale = 1f;
        GameStateManager.Instance.SetState(GameState.Gameplay);

        FadeInSound(transitionData.FadeOutPanelDurationInSec, transitionData.TransitionPanelEase);
        yield return FadeOutTransitionPanel
            (transitionData.FadeOutPanelDurationInSec, transitionData.TransitionPanelEase);
    }

    private IEnumerator FadeInTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = true;

        yield return _fadePanel.DOFade(1f, durationInSec)
            .SetEase(easeType).SetUpdate(true).WaitForCompletion();
    }

    private IEnumerator FadeOutTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = false;
        yield return _fadePanel.DOFade(0f, durationInSec)
            .SetEase(easeType).SetUpdate(true).WaitForCompletion();
    }

    private void FadeOutSound(float durationInSec, Ease easeType)
    {
        VolumeAudioManager.Instance.FadeOutGameplay(durationInSec, easeType);
    }

    private void FadeInSound(float durationInSec, Ease easeType)
    {
        VolumeAudioManager.Instance.InitializeSceneAudio(durationInSec, easeType);
    }

    private IEnumerator PlayCutscenes(CutsceneData[] cutscenes)
    {
        foreach (var cutsceneData in cutscenes)
        {
            _currentCutscenePlaying = StartCoroutine(PlayCutscene(cutsceneData));
            yield return _currentCutscenePlaying;
        }
    }

    private IEnumerator PlayCutscene(CutsceneData cutscene)
    {
        if (cutscene != null)
        {
            yield return new WaitForSecondsRealtime(cutscene.PauseBeforeCutsceneInSec);

            var fadeEase = DOTween.defaultEaseType;
            Tween fadeInAnim = null;
            Tween fadeOutAnim = null;
            bool fadeOutStarted = false;

            void StartFadeIn()
            {
                if (fadeInAnim != null)
                    return;

                var uiCutsceneFadeInDuration = cutscene.UIFadeInDurationInSec;
                fadeInAnim = _fadeCutsceneGroup.DOFade(1f, uiCutsceneFadeInDuration).SetUpdate(true);
                VolumeAudioManager.Instance.FadeInCutscene(uiCutsceneFadeInDuration, fadeEase);
            }

            void StartFadeOut()
            {
                if (fadeOutStarted)
                    return;

                fadeOutStarted = true;
                if (fadeInAnim != null && fadeInAnim.IsActive())
                    fadeInAnim.Kill();
                var uiCutsceneFadeOutDuration = cutscene.UIFadeOutDurationInSec;
                if (uiCutsceneFadeOutDuration <= 0f)
                {
                    _fadeCutsceneGroup.alpha = 0f;
                    VolumeAudioManager.Instance.MuteCutscene();
                    return;
                }

                fadeOutAnim = _fadeCutsceneGroup.DOFade(0f, uiCutsceneFadeOutDuration).SetUpdate(true);
                VolumeAudioManager.Instance.FadeOutCutscene(uiCutsceneFadeOutDuration, fadeEase);
            }

            VolumeAudioManager.Instance.MuteCutscene();

            _currentClipPlaying = StartCoroutine(_cutscenesHandler.PlayCutscene(cutscene, StartFadeIn, StartFadeOut));
            yield return _currentClipPlaying;

            StartFadeOut();
            if (fadeOutAnim != null && fadeOutAnim.IsActive())
                yield return fadeOutAnim.WaitForCompletion();

            _fadeCutsceneGroup.alpha = 0f;
            VolumeAudioManager.Instance.MuteCutscene();

            yield return new WaitForSecondsRealtime(cutscene.PauseAfterCutsceneInSec);
        }
    }
}