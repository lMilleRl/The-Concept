using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private GameObject _rootToDestroyAfterTransition;
    [SerializeField] private Image _fadePanel;
    [SerializeField] private CanvasGroup _fadeCutsceneGroup;
    [SerializeField] private CutsceneAnimationHandler _cutscenesHandler;

    private void OnEnable() => SceneTransitionTrigger.OnTriggerActivated += StartAnimatedTransitionToScene;
    private void OnDisable() => SceneTransitionTrigger.OnTriggerActivated -= StartAnimatedTransitionToScene;

    public void StartAnimatedTransitionToScene(SceneTransitionData transitionData)
    {
        StartCoroutine(TranslateToScene(transitionData));
    }

    private IEnumerator TranslateToScene(SceneTransitionData transitionData)
    {
        StopPlayerUpdate();

        yield return FadeInTransitionPanel
            (transitionData.FadeInTransitionPanelDurationInSec, transitionData.TransitionPanelEase);
        SceneManager.LoadScene(transitionData.SceneName);
        StopPlayerUpdate();
        MuteAmbient();

        yield return PlayCutscenes(transitionData.OwnCutscenesData);

        yield return FadeOutTransitionPanel
            (transitionData.FadeOutTransitionPanelDurationInSec, transitionData.TransitionPanelEase);
        
        Destroy(_rootToDestroyAfterTransition);
        ResumePlayerUpdate();
    }

    private IEnumerator FadeInTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = true;
        FadeOutAmbient(durationInSec);
        yield return _fadePanel.DOFade(1f, durationInSec)
            .SetEase(easeType).WaitForCompletion();
    }

    private IEnumerator FadeOutTransitionPanel(float durationInSec, Ease easeType)
    {
        _fadePanel.raycastTarget = false;
        FadeInAmbient(durationInSec);
        yield return _fadePanel.DOFade(0f, durationInSec)
            .SetEase(easeType).WaitForCompletion();
    }

    private void StopPlayerUpdate()
    {
        var player = FindObjectOfType<Player>();
        player?.StopUpdate();
    }

    private void ResumePlayerUpdate()
    {
        var player = FindObjectOfType<Player>();
        player?.ResumeUpdate();
    }

    private void MuteAmbient()
    {
        var audioManager = FindObjectOfType<AudioManager>();
        audioManager?.MuteAmbient();
    }

    private void FadeOutAmbient(float duration)
    {
        var audioManager = FindObjectOfType<AudioManager>();
        audioManager?.FadeAmbient(0f, duration);
    }

    private void FadeInAmbient(float duration)
    {
        var audioManager = FindObjectOfType<AudioManager>();
        audioManager?.FadeAmbient(1f, duration);
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