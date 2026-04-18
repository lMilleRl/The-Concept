using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "New SceneTransitionData", menuName = "Game/SceneTransitionData")]
public class SceneTransitionData : ScriptableObject
{
    [SerializeField] private string _sceneName;

    [SerializeField] private Ease _transitionPanelEase;
    
    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeInTransitionPanelDurationInSec;
    
    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeOutTransitionPanelDurationInSec;

    [SerializeField] private CutsceneData ownCutsceneData;

    public string SceneName => _sceneName;
    public float FadeInTransitionPanelDurationInSec => _fadeInTransitionPanelDurationInSec;
    public float FadeOutTransitionPanelDurationInSec => _fadeOutTransitionPanelDurationInSec;
    public Ease TransitionPanelEase => _transitionPanelEase;
    public CutsceneData OwnCutsceneData => ownCutsceneData;
}