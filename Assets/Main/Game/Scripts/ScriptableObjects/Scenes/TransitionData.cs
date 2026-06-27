using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New SceneTransitionData", menuName = "Game/SceneTransitionData")]
public class TransitionData : ScriptableObject
{
    [SerializeField] private string _sceneName;

    [SerializeField] private Ease _transitionPanelEase;

    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeInPanelDurationInSec = 1;
    
    [Range(0, float.MaxValue)] [SerializeField]
    private float _fadeOutPanelDurationInSec = 3;

    [Tooltip("Пауза после затухания экрана перед загрузкой сцены. Даёт доиграть звукам триггера (шаги, дверь).")]
    [Range(0, float.MaxValue)] [SerializeField]
    private float _pauseBeforeLoadInSec;
    
    [SerializeField] private CutsceneData[] ownCutscenesData;

    public string SceneName => _sceneName;
    public float FadeInPanelDurationInSec => _fadeInPanelDurationInSec;
    public float FadeOutPanelDurationInSec => _fadeOutPanelDurationInSec;
    public float PauseBeforeLoadInSec => _pauseBeforeLoadInSec;
    public Ease TransitionPanelEase => _transitionPanelEase;
    public CutsceneData[] OwnCutscenesData => ownCutscenesData;
}