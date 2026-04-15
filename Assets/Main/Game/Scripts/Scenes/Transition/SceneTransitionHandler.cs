using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionHandler : MonoBehaviour
{
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
        
        yield return _fadePanel.DOFade(1f, transitionData.FadeInTransitionPanelDurationInSec)
            .SetEase(transitionData.TransitionPanelEase).WaitForCompletion();
        SceneManager.LoadScene(transitionData.SceneIndex);
        
        yield return PlayCutscene(transitionData.OwnCutsceneData);
        
        _fadePanel.DOFade(0f, transitionData.FadeOutTransitionPanelDurationInSec)
            .SetEase(transitionData.TransitionPanelEase);

        ResumePlayerUpdate();
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