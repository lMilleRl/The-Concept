using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private CutsceneAnimationHandler _cutscenesHandler;
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private Player _player;


    private void OnEnable() => SceneTransitionTrigger.OnTriggerActivated += OnSceneTransition;
    private void OnDisable() => SceneTransitionTrigger.OnTriggerActivated -= OnSceneTransition;

    private void OnSceneTransition(SceneTransitionData transitionData)
    {
        StartCoroutine(TranslateToNextScene(transitionData));
    }

    private IEnumerator TranslateToNextScene(SceneTransitionData transitionData)
    {
        _player.StopUpdate();
        var fadeOutAnim = _fadeGroup.DOFade(1f, transitionData.FadeDurationInSec);
        yield return fadeOutAnim.WaitForCompletion();
        SceneManager.LoadScene(transitionData.SceneIndex);

        if (transitionData.OwnCutsceneData != null)
            yield return _cutscenesHandler.PlayCutscene(transitionData.OwnCutsceneData);

        var fadeInAnim = _fadeGroup.DOFade(0f, transitionData.FadeDurationInSec);
        yield return fadeInAnim.WaitForCompletion();

        _player.ResumeUpdate();
    }
}