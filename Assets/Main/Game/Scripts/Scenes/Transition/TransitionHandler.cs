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

        FadeOutSound(transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);
        yield return FadeInTransitionPanel
            (transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);

        // Пауза после затухания экрана: даём доиграть звукам триггера (дверь, шаги) до уничтожения сцены
        yield return new WaitForSeconds(transitionData.PauseBeforeLoadInSec);

        if (!string.IsNullOrEmpty(transitionData.SceneName))
        {
            SceneManager.LoadScene(transitionData.SceneName);
            GameStateManager.Instance.SetState(GameState.PassiveShow);
            VolumeAudioManager.Instance.MuteGameplay();
        }

        yield return PlayCutscenes(transitionData.OwnCutscenesData);

        GameStateManager.Instance.SetState(GameState.Gameplay);

        FadeInSound(transitionData.FadeOutPanelDurationInSec, transitionData.TransitionPanelEase);
        yield return FadeOutTransitionPanel
            (transitionData.FadeOutPanelDurationInSec, transitionData.TransitionPanelEase);
    }

    private IEnumerator FadeInTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = true;
        
        yield return _fadePanel.DOFade(1f, durationInSec)
            .SetEase(easeType).WaitForCompletion();
    }

    private IEnumerator FadeOutTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = false;
        yield return _fadePanel.DOFade(0f, durationInSec)
            .SetEase(easeType).WaitForCompletion();
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
            yield return new WaitForSeconds(cutscene.PauseBeforeCutsceneInSec);

            var fadeEase = DOTween.defaultEaseType;
            Tween fadeInAnim = null;
            Tween fadeOutAnim = null;

            void StartFadeIn()
            {
                if (fadeInAnim != null)
                    return;

                var uiCutsceneFadeInDuration = cutscene.UIFadeInDurationInSec;
                fadeInAnim = _fadeCutsceneGroup.DOFade(1f, uiCutsceneFadeInDuration);
                VolumeAudioManager.Instance.FadeInCutscene(uiCutsceneFadeInDuration, fadeEase);
            }

            void StartFadeOut()
            {
                if (fadeOutAnim != null)
                    return;

                fadeInAnim?.Kill();
                var uiCutsceneFadeOutDuration = cutscene.UIFadeOutDurationInSec;
                fadeOutAnim = _fadeCutsceneGroup.DOFade(0f, uiCutsceneFadeOutDuration);
                VolumeAudioManager.Instance.FadeOutCutscene(uiCutsceneFadeOutDuration, fadeEase);
            }

            VolumeAudioManager.Instance.MuteCutscene();

            _currentClipPlaying = StartCoroutine(_cutscenesHandler.PlayCutscene(cutscene, StartFadeIn, StartFadeOut));
            yield return _currentClipPlaying;

            StartFadeOut();
            yield return fadeOutAnim.WaitForCompletion();
            VolumeAudioManager.Instance.MuteCutscene();

            yield return new WaitForSeconds(cutscene.PauseAfterCutsceneInSec);
        }
    }
}