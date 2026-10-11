using System;
using TextBox;
using UnityEngine;
using UnityEngine.Events;

namespace Choices
{
    [CreateAssetMenu(fileName = "Choice Sequence", menuName = "Game/Choice Sequence")]
    public class ChoiceSequenceData : ScriptableObject
    {
        [SerializeField] private TextBoxData _beforeChoiceText;
        [SerializeField] private bool _showQuestion;
        [SerializeField] private TextBoxData _questionText;
        [SerializeField, Min(1)] private int _gridColumns = 1;
        [SerializeField] private ChoiceOptionData[] _options;

        public TextBoxData BeforeChoiceText => _beforeChoiceText;
        public bool ShowQuestion => _showQuestion && _questionText != null && !string.IsNullOrWhiteSpace(_questionText.Text);
        public TextBoxData QuestionText => _questionText;
        public int GridColumns => Mathf.Max(1, _gridColumns);
        public ChoiceOptionData[] Options => _options ?? Array.Empty<ChoiceOptionData>();
    }

    [Serializable]
    public class ChoiceOptionData
    {
        [SerializeField] private int _id;
        [SerializeField] private string _label;
        [SerializeField] private TextBoxData _afterChoiceText;
        [SerializeField] private ChoiceOptionActionOrder _actionOrder;

        public int Id => _id;
        public string Label => _label;
        public TextBoxData AfterChoiceText => _afterChoiceText;
        public ChoiceOptionActionOrder ActionOrder => _actionOrder;
    }

    public enum ChoiceOptionActionOrder
    {
        TextThenEvent,
        EventThenText
    }

    [Serializable]
    public class ChoiceOptionEventBinding
    {
        [SerializeField] private int _optionId;
        [SerializeField] private UnityEvent _onSelected;

        public int OptionId => _optionId;
        public void Invoke() => _onSelected?.Invoke();
    }
}