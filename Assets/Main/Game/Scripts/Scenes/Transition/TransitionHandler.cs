using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour, ITransitionHandler
{
    public static ITransitionHandler Instance;
    
    [SerializeField] private Image _fadePanel;
    [SerializeField] private CanvasGroup _fadeCutsceneGroup;
    [SerializeField] private CutscenePlayer _cutscenesHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }

    public void StartTransition(TransitionData transitionData)
    {
        StartCoroutine(Translate(transitionData));
    }

    private IEnumerator Translate(TransitionData transitionData)
    {
        GameStateManager.Instance.SetState(GameState.PassiveShow);

        FadeOutSound(transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);
        _fadePanel.color = transitionData.FadePanelColorInFadeIn;
        yield return FadeInTransitionPanel
            (transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);

        // Пауза после затухания экрана: даём доиграть звукам триггера (дверь, шаги) до уничтожения сцены
        yield return new WaitForSeconds(transitionData.PauseBeforeLoadInSec);

        if (!string.IsNullOrEmpty(transitionData.SceneName))
        {
            SceneManager.LoadScene(transitionData.SceneName);
            VolumeAudioManager.Instance.MuteGameplay();
        }

        yield return PlayCutscenes(transitionData.OwnCutscenesData);

        GameStateManager.Instance.SetState(GameState.Gameplay);
        
        FadeInSound(transitionData.FadeInPanelDurationInSec, transitionData.TransitionPanelEase);
        _fadePanel.color = transitionData.FadePanelColorInFadeOut;
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
            yield return PlayCutscene(cutsceneData);
    }

    private IEnumerator PlayCutscene(CutsceneData cutscene)
    {
        if (cutscene != null)
        {
            yield return new WaitForSeconds(cutscene.PauseBeforeCutsceneInSec);

            var uiCutsceneFadeInDuration = cutscene.UIFadeInDurationInSec;
            _fadeCutsceneGroup.DOFade(1f, uiCutsceneFadeInDuration);

            yield return _cutscenesHandler.PlayCutscene(cutscene);

            var uiCutsceneFadeOutDuration = cutscene.UIFadeOutDurationInSec;
            var fadeOutAnim = _fadeCutsceneGroup.DOFade(0f, uiCutsceneFadeOutDuration);
            yield return fadeOutAnim.WaitForCompletion();

            yield return new WaitForSeconds(cutscene.PauseAfterCutsceneInSec);
        }
    }
}