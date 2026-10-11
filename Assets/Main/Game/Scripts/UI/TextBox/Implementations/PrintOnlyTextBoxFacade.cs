using System;
using UnityEngine;

namespace TextBox
{
    public class PrintOnlyTextBoxFacade : ITextBoxFacade, IDisposable
    {
        private const float FallbackCharsPerSecond = 20f;

        private readonly ITextBoxUI _ui;
        private readonly ITypeRunner _typeRunner;
        private readonly ICommandParser _commandParser;
        private readonly ITextBoxVoiceSpeaker _voiceSpeaker;
        private readonly ITextFormChanger _textFormChanger;
        private readonly TextStyleProfile _defaultStyle;

        private bool _isPrinting;

        public event Action OnPageStarted;
        public event Action OnPageFinished;
        public event Action OnCurrentTextEnded;
        public event Action OnHidden;

        public PrintOnlyTextBoxFacade(
            ITextBoxUI ui,
            ITypeRunner typeRunner,
            ICommandParser commandParser,
            ITextBoxVoiceSpeaker voiceSpeaker,
            ITextFormChanger textFormChanger,
            TextStyleProfile defaultStyle)
        {
            _ui = ui;
            _typeRunner = typeRunner;
            _commandParser = commandParser;
            _voiceSpeaker = voiceSpeaker;
            _textFormChanger = textFormChanger;
            _defaultStyle = defaultStyle;

            _typeRunner.OnPageStarted += HandlePageStarted;
            _typeRunner.OnPageFinished += HandlePageFinished;
            _typeRunner.OnTextFinished += HandleTextFinished;
        }

        public void Dispose()
        {
            Hide();
            _typeRunner.OnPageStarted -= HandlePageStarted;
            _typeRunner.OnPageFinished -= HandlePageFinished;
            _typeRunner.OnTextFinished -= HandleTextFinished;
            _ui.Dispose();
        }

        public void Show(TextBoxData data)
        {
            if (_isPrinting)
            {
                Debug.LogWarning($"[{nameof(PrintOnlyTextBoxFacade)}] A print is already active.");
                return;
            }

            StartTyping(data, 0);
        }

        public void ReplaceText(TextBoxData data, int startCharIndex)
        {
            OnCurrentTextEnded?.Invoke();
            StartTyping(data, startCharIndex);
        }

        public void Hide()
        {
            if (!_isPrinting)
                return;

            _typeRunner.Stop();
            FinishPrinting();
        }

        public void TryTurnPage()
        {
            _typeRunner.TurnToNextPage();
        }

        private void StartTyping(TextBoxData data, int startCharIndex)
        {
            if (data == null)
            {
                Hide();
                return;
            }

            _typeRunner.Stop();
            _isPrinting = true;

            var parseResult = _commandParser.Init(data.Text);
            _ui.InitText(parseResult.CleanText, startCharIndex);
            _textFormChanger.SetText(_ui);
            _textFormChanger.ClearAll();
            _voiceSpeaker.SetProfile(data.Voice);
            _voiceSpeaker.Resume();
            _typeRunner.SetPosition(_ui, startCharIndex);

            var style = data.Style != null ? data.Style : _defaultStyle;
            if (style != null)
            {
                _ui.ContentText.color = style.DefaultColor;
                _ui.ContentText.fontSize = style.DefaultFontSize;
                if (style.DefaultFont != null)
                    _ui.ContentText.font = style.DefaultFont;

                _typeRunner.SetSpeed(style.DefaultSpeed);
            }
            else
            {
                _typeRunner.SetSpeed(FallbackCharsPerSecond);
            }

            _typeRunner.Run(_ui);
        }

        private void HandlePageStarted()
        {
            OnPageStarted?.Invoke();
        }

        private void HandlePageFinished()
        {
            OnPageFinished?.Invoke();
            _typeRunner.TurnToNextPage();
        }

        private void HandleTextFinished()
        {
            FinishPrinting();
        }

        private void FinishPrinting()
        {
            if (!_isPrinting)
                return;

            _isPrinting = false;
            _voiceSpeaker.Mute();
            _textFormChanger.ClearAll();
            OnCurrentTextEnded?.Invoke();
            OnHidden?.Invoke();
        }
    }
}