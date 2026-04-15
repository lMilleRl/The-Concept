using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "New SceneTransitionData", menuName = "Game/SceneTransitionData")]
public class SceneTransitionData : ScriptableObject
{
    [SerializeField] private int _sceneIndex;

    [SerializeField] private Ease _transitionPanelEase;
    
    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeInTransitionPanelDurationInSec;
    
    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeOutTransitionPanelDurationInSec;

    [SerializeField] private CutsceneData ownCutsceneData;

    public void OnValidate()
    {
        _sceneIndex = Mathf.Max(_sceneIndex, 0);
    }

    public int SceneIndex => _sceneIndex;
    public float FadeInTransitionPanelDurationInSec => _fadeInTransitionPanelDurationInSec;
    public float FadeOutTransitionPanelDurationInSec => _fadeOutTransitionPanelDurationInSec;
    public Ease TransitionPanelEase => _transitionPanelEase;
    public CutsceneData OwnCutsceneData => ownCutsceneData;
}