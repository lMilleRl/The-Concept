using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextBoxHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private KeyCode _nextPageKey;

    public UnityEvent OnShow;
    public UnityEvent OnEndPages;

    public void Show(TextForTextBox description)
    {
        OnShow?.Invoke();

        _text.overflowMode = TextOverflowModes.Page;
        _text.text = description.OwnText;
        _text.fontSize = description.FontSize;

        _text.ForceMeshUpdate();

        StartCoroutine(TurnPages(description));
    }

    private IEnumerator TurnPages(TextForTextBox description)
    {
        for (int i = 1; i <= _text.textInfo.pageCount; i++)
        {
            _text.pageToDisplay = i;
            var textAppearAnim = _text.DOPage(_text.textInfo.pageInfo[i - 1], description.TextAppearDuration)
                .SetEase(description.EaseTextTweenType);
            yield return new DOTweenCYInstruction.WaitForCompletion(textAppearAnim);
            yield return new WaitUntil(() => Input.GetKeyDown(_nextPageKey));
        }
        
        OnEndPages?.Invoke();
    }
}