using TMPro;
using UnityEngine;

namespace TextBox
{
    [CreateAssetMenu(fileName = "New TextStyleProfile", menuName = "TextBox/TextStyleProfile")]
    public class TextStyleProfile : ScriptableObject
    {
        [SerializeField] private float _defaultSpeed = 20f;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private float _defaultFontSize = 36f;
        [SerializeField] private TMP_FontAsset _defaultFont;

        public float DefaultSpeed => _defaultSpeed;
        public Color DefaultColor => _defaultColor;
        public float DefaultFontSize => _defaultFontSize;
        public TMP_FontAsset DefaultFont => _defaultFont;
    }
}
