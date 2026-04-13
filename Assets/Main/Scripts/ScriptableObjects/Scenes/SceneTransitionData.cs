using UnityEngine;

[CreateAssetMenu(fileName = "New SceneTransitionData", menuName = "Game/SceneTransitionData")]
public class SceneTransitionData : ScriptableObject
{
    [Range(0, int.MaxValue)] [SerializeField]
    private int _sceneIndex;
    
    [Range(0, int.MaxValue)] [SerializeField]
    private float _fadeDurationInSec;
    
    [SerializeField] private CutsceneData ownCutsceneData;
    
    public int SceneIndex => _sceneIndex;
    public float FadeDurationInSec => _fadeDurationInSec;
    public CutsceneData OwnCutsceneData => ownCutsceneData;
}
