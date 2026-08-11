using System;
using System.Collections;
using UnityEngine;

namespace TextBox
{
    public class TextBoxFacade : ITextBoxFacade
    {
        public event Action OnHidden;

        private readonly ITextBoxUI _ui;
        private readonly ITypeRunner _typeRunner;
        private readonly ICommandParser _commandParser;
        private readonly ITextBoxVoiceSpeaker _voiceSpeaker;
        private readonly ITextFormChanger _textFormChanger;
        private readonly ITextBoxInput _input;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly TextStyleProfile _defaultStyle;

        private bool _canTurnPage;
        private bool _autoPlay;
        private float _autoPagePause;
        private Coroutine _autoTurnCoroutine;
        private BoardTransitionContext _hideTransition;
        
        public TextBoxFacade(TextBoxFacadeData data)
        {
            _ui = data.UI;
            _typeRunner = data.TypeRunner;
            _commandParser = data.CommandParser;
            _voiceSpeaker = data.VoiceSpeaker;
            _textFormChanger = data.TextFormChanger;
            _input = data.Input;
            _coroutineRunner = data.CoroutineRunner;
            _defaultStyle = data.DefaultStyle;

            _typeRunner.OnTextFinished += Hide;
            _typeRunner.OnPageFinished += ResumeTurningPage;
            _input.OnTurnPagePressed += TryTurnPage;
        }
        
        public void Show(TextBoxData data)
        {
            _typeRunner.Stop();
            StopAutoTurn();

            _autoPlay = data.AutoPlay;
            _autoPagePause = data.AutoPagePause;

            _hideTransition = data.HideTransition;

            InitUI(data);
            _ui.ShowBoard(data.ShowTransition);

            _textFormChanger.SetText(_ui);
            _textFormChanger.ClearAll();
            _commandParser.Init(_ui.GetTextInfo());
            _voiceSpeaker.SetProfile(data.Voice);
            _voiceSpeaker.Resume();
            _input.Enable();
            
            
            var style = data.Style != null ? data.Style : _defaultStyle;
            _typeRunner.SetSpeed(style.DefaultSpeed);
            _typeRunner.Run(_ui);
        }

        public void Hide()
        {
            StopAutoTurn();
            _typeRunner.Stop();
            _voiceSpeaker.Mute();
            _textFormChanger.ClearAll();
            _ui.HideBoard(_hideTransition);
            _input.Disable();
            _canTurnPage = false;
            OnHidden?.Invoke();
        }

        public void TryTurnPage()
        {
            if (_canTurnPage)
            {
                StopAutoTurn();
                _typeRunner.TurnToNextPage();
                _canTurnPage = false;
            }
        }

        private void ResumeTurningPage()
        {
            _canTurnPage = true;

            if (_autoPlay)
                _autoTurnCoroutine = _coroutineRunner.StartCoroutine(AutoTurnPage());
        }

        private IEnumerator AutoTurnPage()
        {
            yield return new WaitForSeconds(_autoPagePause);
            TryTurnPage();
        }

        private void InitUI(TextBoxData data)
        {
            var style = data.Style != null ? data.Style : _defaultStyle;

            _ui.ContentText.color = style.DefaultColor;
            _ui.ContentText.fontSize = style.DefaultFontSize;

            if (style.DefaultFont != null)
                _ui.ContentText.font = style.DefaultFont;

            _ui.SetText(data.Text);
        }

        private void StopAutoTurn()
        {
            if (_autoTurnCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_autoTurnCoroutine);
                _autoTurnCoroutine = null;
            }
        }
    }
}
