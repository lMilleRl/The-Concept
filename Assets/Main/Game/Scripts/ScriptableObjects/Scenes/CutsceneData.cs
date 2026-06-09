using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New CutsceneData", menuName = "Game/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [SerializeField] private VideoClip _clip;
    [Range(0f, float.MaxValue)] [SerializeField] private float _uiFadeInDurationInSec;
    [Range(0f, float.MaxValue)] [SerializeField] private float _uiFadeOutDurationInSec;
    
    [Range(0f, float.MaxValue)] [SerializeField] private float _pauseBeforeCutsceneInSec;
    [Range(0f, float.MaxValue)] [SerializeField] private float _pauseAfterCutsceneInSec;

    public VideoClip Clip => _clip;
    
    public float UIFadeInDurationInSec => _uiFadeInDurationInSec;
    public float UIFadeOutDurationInSec => _uiFadeOutDurationInSec;
    
    public float PauseAfterCutsceneInSec => _pauseAfterCutsceneInSec;
    public float PauseBeforeCutsceneInSec => _pauseBeforeCutsceneInSec;
}
