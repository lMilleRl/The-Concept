using System;
using System.Globalization;
using TextBox;
using UnityEngine;

public class TextSequencePlayer : MonoBehaviour
{
    [Serializable]
    private class TimedText
    {
        [TextArea] public string Text;
        [Min(0.01f)] public float Duration = 1f;
    }

    [SerializeField] private TimedText[] _texts;
    [SerializeField] private VoiceProfile _voice;
    [SerializeField] private TextStyleProfile _style;
    [SerializeField] private BoardTransitionContext _showTransition;
    [SerializeField] private BoardTransitionContext _hideTransition;

    private TextBoxFacadeMono _facadeMono;
    private TextBoxFacade _facade;
    private int _nextTextIndex;
    private bool _isPlaying;

    private void Start()
    {
        Play();
    }

    [ContextMenu("Play Text Sequence")]
    public void Play()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning($"[{nameof(TextSequencePlayer)}] The sequence can only be played in Play Mode.", this);
            return;
        }

        if (_isPlaying)
        {
            Debug.LogWarning($"[{nameof(TextSequencePlayer)}] The sequence is already playing.", this);
            return;
        }

        _facadeMono = TextBoxFacadeMono.Instance;
        _facade = _facadeMono != null ? _facadeMono.Facade : null;

        if (_facade == null)
        {
            Debug.LogWarning($"[{nameof(TextSequencePlayer)}] TextBoxFacade is not initialized.", this);
            return;
        }

        _nextTextIndex = 0;
        _isPlaying = true;
        _facade.OnHidden += HandleTextHidden;
        ShowNextText();
    }

    private void OnDisable()
    {
        StopSequence();
    }

    private void HandleTextHidden()
    {
        if (_isPlaying)
            ShowNextText();
    }

    private void ShowNextText()
    {
        while (_texts != null && _nextTextIndex < _texts.Length)
        {
            int currentIndex = _nextTextIndex++;
            TimedText timedText = _texts[currentIndex];

            if (timedText == null || string.IsNullOrEmpty(timedText.Text))
            {
                Debug.LogWarning($"[{nameof(TextSequencePlayer)}] Text at index {currentIndex} is empty and was skipped.", this);
                continue;
            }

            if (timedText.Duration <= 0f)
            {
                Debug.LogWarning($"[{nameof(TextSequencePlayer)}] Duration at index {currentIndex} must be greater than zero. Text was skipped.", this);
                continue;
            }

            float charsPerSecond = timedText.Text.Length / timedText.Duration;
            string speed = charsPerSecond.ToString("R", CultureInfo.InvariantCulture);
            var data = new TextBoxData
            {
                Text = $"[speed={speed}]{timedText.Text}",
                Voice = _voice,
                AutoPlay = false,
                AutoPagePause = 0f,
                Style = _style,
                ShowTransition = _showTransition,
                HideTransition = _hideTransition
            };

            _facadeMono.Show(data);
            return;
        }

        StopSequence();
    }

    private void StopSequence()
    {
        if (_facade != null)
            _facade.OnHidden -= HandleTextHidden;

        _isPlaying = false;
        _facade = null;
        _facadeMono = null;
    }
}
