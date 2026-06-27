using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "New TextForTextBox", menuName = "Game/TextForTextBox")]
public class TextForTextBox : ScriptableObject
{
    private const int MinFontSize = 1;
    
    [SerializeField] private string _ownText;
    [SerializeField] private int _fontSize = 56;
    [Range(0f, float.MaxValue)] [SerializeField] private float _textAppearDuration = 2f;
    [SerializeField] private Ease _easeTextTweenType = Ease.Linear;
    [SerializeField] private bool _isAutoPlay;
    [Range(0f, float.MaxValue)] [SerializeField] private float _pauseBetweenPagesInSec = 1f;

    private void OnValidate()
    {
        _fontSize = Mathf.Max(MinFontSize, _fontSize);
    }

    public string OwnText => _ownText;
    public int FontSize => _fontSize;
    public float TextAppearDuration => _textAppearDuration;
    public Ease EaseTextTweenType => _easeTextTweenType;
    public bool IsAutoPlay => _isAutoPlay;
    public float PauseBetweenPagesInSec => _pauseBetweenPagesInSec;
}
