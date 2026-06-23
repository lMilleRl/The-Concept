using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextBoxHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _isHasOwnDuration;
    [SerializeField] private KeyCode _nextPageKey;

    public UnityEvent OnShow;
    public UnityEvent OnEndPages;
    
    public void SetText(TextForTextBox text)
    {
        _text.overflowMode = TextOverflowModes.Page;
        _text.text = text.OwnText;
        _text.fontSize = text.FontSize;

        _text.ForceMeshUpdate();
    }
    
    public void ShowText(TextForTextBox text)
    {
        OnShow?.Invoke();
        GameStateManager.Instance.SetState(GameState.TextBox);

        SetText(text);

        StartCoroutine(TurnPages(text));
    }

    private IEnumerator TurnPages(TextForTextBox description)
    {
        var waitForKey = new WaitUntil(() => Input.GetKeyDown(_nextPageKey));
        for (int i = 1; i <= _text.textInfo.pageCount; i++)
        {
            _text.pageToDisplay = i;
            var textAppearAnim = _text.DOPage(_text.textInfo.pageInfo[i - 1], description.TextAppearDuration)
                .SetEase(description.EaseTextTweenType);
            yield return textAppearAnim.WaitForCompletion();
            yield return waitForKey;
        }
        
        OnEndPages?.Invoke();
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
}