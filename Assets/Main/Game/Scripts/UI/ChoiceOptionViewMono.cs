using TMPro;
using TextBox;
using UnityEngine;

namespace Choices
{
    public class ChoiceOptionViewMono : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private GameObject _selectionIndicator;

        public void Show(string label, bool isSelected, TextStyleProfile styleProfile)
        {
            if (_label != null)
            {
                _label.text = label;
                if (styleProfile != null)
                {
                    _label.color = styleProfile.DefaultColor;
                    _label.fontSize = styleProfile.DefaultFontSize;
                    if (styleProfile.DefaultFont != null)
                        _label.font = styleProfile.DefaultFont;
                }
            }

            SetSelected(isSelected);
            gameObject.SetActive(true);
        }

        public void SetSelected(bool isSelected)
        {
            if (_selectionIndicator != null)
                _selectionIndicator.SetActive(isSelected);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}