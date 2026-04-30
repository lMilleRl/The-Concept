using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "New CutsceneData", menuName = "Game/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Ease _scrollSpriteEase;
    [SerializeField] private bool _isScrolling;
    [Range(0f, float.MaxValue)] [SerializeField] private float scrollDurationInSec;
    [SerializeField] private TextForTextBox _text;
    [Range(0f, float.MaxValue)] [SerializeField] private float _pausePerPageInSec;
    
    [Range(0f, float.MaxValue)] [SerializeField] private float _uiFadeInDurationInSec;
    [Range(0f, float.MaxValue)] [SerializeField] private float _uiFadeOutDurationInSec;
    
    [Range(0f, float.MaxValue)] [SerializeField] private float _pauseBeforeCutsceneInSec;
    [Range(0f, float.MaxValue)] [SerializeField] private float _pauseAfterCutsceneInSec;
    
    public Sprite OwnSprite => _sprite;
    public bool IsScrolling => _isScrolling;
    public float ScrollDurationInSec => scrollDurationInSec;
    public Ease ScrollSpriteEase => _scrollSpriteEase;
    public TextForTextBox OwnTextInfo => _text;
    public float PausePerPageInSec => _pausePerPageInSec;
    
    public float UIFadeInDurationInSec => _uiFadeInDurationInSec;
    public float UIFadeOutDurationInSec => _uiFadeOutDurationInSec;
    
    public float PauseAfterCutsceneInSec => _pauseAfterCutsceneInSec;
    public float PauseBeforeCutsceneInSec => _pauseBeforeCutsceneInSec;
}
