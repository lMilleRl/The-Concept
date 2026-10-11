using System.Collections.Generic;
using TMPro;
using TextBox;
using UnityEngine;
using UnityEngine.UI;

namespace Choices
{
    public class ChoiceMenuViewMono : MonoBehaviour
    {
        [SerializeField] private ChoicePanelMode _panelMode;
        [SerializeField] private TextBoxBoard _textBoxBoard;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TextStyleProfile _optionTextStyleProfile;
        [SerializeField] private TMP_Text _textBoxContentText;
        [SerializeField] private GridLayoutGroup _optionsLayout;
        [SerializeField] private ChoiceOptionViewMono _optionViewPrefab;

        private readonly List<ChoiceOptionViewMono> _optionViews = new();
        private bool _questionTextWasEnabled;
        private bool _hasQuestionTextState;
        private bool _textBoxContentWasEnabled;
        private bool _hasTextBoxContentState;

        public TMP_Text QuestionText => _questionText;

        public bool Show(ChoiceSequenceData data, int selectedIndex)
        {
            if (data == null || data.Options.Length == 0)
            {
                Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] Choice data is null or contains no options.", this);
                return false;
            }

            if (_optionsLayout == null || _optionViewPrefab == null)
            {
                Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] Assign a GridLayoutGroup and option view prefab.", this);
                return false;
            }

            if (_panelMode == ChoicePanelMode.TextBoxBoard && (_textBoxBoard == null || _textBoxContentText == null))
            {
                Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] TextBoxBoard mode requires the board and regular TextBox content TMP_Text.", this);
                return false;
            }

            if (_panelMode == ChoicePanelMode.CanvasGroup && _canvasGroup == null)
            {
                Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] CanvasGroup mode requires a CanvasGroup.", this);
                return false;
            }

            if (data.ShowQuestion && _questionText == null)
            {
                Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] Question text is enabled but no TMP_Text is assigned.", this);
                return false;
            }

            for (int i = 0; i < data.Options.Length; i++)
            {
                if (data.Options[i] == null)
                {
                    Debug.LogError($"[{nameof(ChoiceMenuViewMono)}] Choice option at index {i} is null.", this);
                    return false;
                }
            }

            if (_panelMode == ChoicePanelMode.TextBoxBoard && _textBoxContentText != _questionText)
            {
                _textBoxContentWasEnabled = _textBoxContentText.enabled;
                _hasTextBoxContentState = true;
                _textBoxContentText.text = string.Empty;
                _textBoxContentText.maxVisibleCharacters = 0;
                _textBoxContentText.enabled = false;
            }

            if (_questionText != null)
            {
                _questionTextWasEnabled = _questionText.enabled;
                _hasQuestionTextState = true;
                _questionText.enabled = data.ShowQuestion;
                _questionText.text = string.Empty;
            }

            _optionsLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _optionsLayout.constraintCount = data.GridColumns;
            _optionsLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            _optionsLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            EnsureOptionViews(data.Options.Length);

            for (int i = 0; i < _optionViews.Count; i++)
            {
                if (i < data.Options.Length)
                    _optionViews[i].Show(data.Options[i].Label, i == selectedIndex, _optionTextStyleProfile);
                else
                    _optionViews[i].Hide();
            }

            ShowPanel();
            return true;
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            for (int i = 0; i < _optionViews.Count; i++)
            {
                if (_optionViews[i] != null)
                    _optionViews[i].SetSelected(i == selectedIndex);
            }
        }

        private void EnsureOptionViews(int count)
        {
            while (_optionViews.Count < count)
            {
                var optionView = Instantiate(_optionViewPrefab, _optionsLayout.transform, false);
                optionView.Hide();
                _optionViews.Add(optionView);
            }
        }

        private void ShowPanel()
        {
            if (_panelMode == ChoicePanelMode.TextBoxBoard)
            {
                _textBoxBoard.Show(default);
                return;
            }

            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void HidePanel()
        {
            if (_panelMode == ChoicePanelMode.TextBoxBoard)
            {
                _textBoxBoard?.Hide(default);
                return;
            }

            if (_canvasGroup == null)
                return;

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Hide()
        {
            HidePanel();

            for (int i = 0; i < _optionViews.Count; i++)
                _optionViews[i]?.Hide();

            if (_questionText != null)
            {
                _questionText.text = string.Empty;
                if (_hasQuestionTextState)
                    _questionText.enabled = _questionTextWasEnabled;
                _hasQuestionTextState = false;
            }

            if (_textBoxContentText != null && _hasTextBoxContentState)
            {
                _textBoxContentText.enabled = _textBoxContentWasEnabled;
                _hasTextBoxContentState = false;
            }
        }
    }

    public enum ChoicePanelMode
    {
        TextBoxBoard,
        CanvasGroup
    }
}