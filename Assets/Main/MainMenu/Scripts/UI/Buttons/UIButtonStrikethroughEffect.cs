using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIButtonStrikethroughEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _line;
    [Min(0f)] [SerializeField] private float _horizontalPadding = 4f;
    [Min(0f)] [SerializeField] private float _thickness = 2f;
    [SerializeField] private float _verticalOffset;

    private RectTransform _lineRect;

    private void Start()
    {
        RefreshLine();
    }

    public void RefreshLine()
    {
        Canvas.ForceUpdateCanvases();
        _text.ForceMeshUpdate();

        TMP_TextInfo textInfo = _text.textInfo;
        int firstVisibleCharacter = -1;
        int lastVisibleCharacter = -1;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                continue;
            }

            if (firstVisibleCharacter < 0)
            {
                firstVisibleCharacter = i;
            }

            lastVisibleCharacter = i;
        }

        if (firstVisibleCharacter < 0)
        {
            return;
        }

        TMP_CharacterInfo first = textInfo.characterInfo[firstVisibleCharacter];
        TMP_CharacterInfo last = textInfo.characterInfo[lastVisibleCharacter];
        float lineY = first.baseLine
            + first.fontAsset.faceInfo.strikethroughOffset * first.scale
            + _verticalOffset;
        Vector3 startInText = new Vector3(first.bottomLeft.x, lineY, 0f);
        Vector3 endInText = new Vector3(last.topRight.x, lineY, 0f);
        RectTransform textRect = _text.rectTransform;

        _lineRect = _line.rectTransform;
        RectTransform lineParent = (RectTransform)_lineRect.parent;
        Vector3 startInParent = lineParent.InverseTransformPoint(
            textRect.TransformPoint(startInText));
        Vector3 endInParent = lineParent.InverseTransformPoint(
            textRect.TransformPoint(endInText));
        Vector2 lineDirection = endInParent - startInParent;

        _lineRect.anchorMin = Vector2.one * 0.5f;
        _lineRect.anchorMax = Vector2.one * 0.5f;
        _lineRect.pivot = Vector2.one * 0.5f;
        _lineRect.localScale = Vector3.one;
        _lineRect.localRotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(lineDirection.y, lineDirection.x) * Mathf.Rad2Deg);
        _lineRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            lineDirection.magnitude + _horizontalPadding);
        _lineRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _thickness);
        _lineRect.anchoredPosition = (Vector2)((startInParent + endInParent) * 0.5f)
            - lineParent.rect.center;

        _line.color = _text.color;
        _line.raycastTarget = false;
    }
}
