using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "New TextForTextBox", menuName = "Game/TextForTextBox")]
public class TextForTextBox : ScriptableObject
{
    private const int MinFontSize = 1;
    
    [SerializeField] private string _ownText;
    [SerializeField] private int _fontSize;
    [Range(0f, float.MaxValue)] [SerializeField] private float _textAppearDuration;
    [SerializeField] private Ease _easeTextTweenType;

    private void OnValidate()
    {
        _fontSize = Mathf.Max(MinFontSize, _fontSize);
    }

    public string OwnText => _ownText;
    public int FontSize => _fontSize;
    public float TextAppearDuration => _textAppearDuration;
    public Ease EaseTextTweenType => _easeTextTweenType;
}
