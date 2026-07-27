using System.Collections;
using UnityEngine;

namespace TextBox
{
    public class TextBoxFacade : ITextBoxFacade
    {
        private readonly ITextBoxUI _ui;
        private readonly ITypeRunner _typeRunner;
        private readonly ICommandParser _commandParser;
        private readonly ITextBoxVoiceSpeaker _voiceSpeaker;
        private readonly ITextChanger _textChanger;
        private readonly ITextBoxInput _input;
        private readonly ICoroutineRunner _coroutineRunner;

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
            _textChanger = data.TextChanger;
            _input = data.Input;
            _coroutineRunner = data.CoroutineRunner;

            _typeRunner.OnTextFinished += Hide;
            _typeRunner.OnPageFinished += ResumeTurningPage;
            _input.OnTurnPagePressed += TryTurnPage;
        }
        
        public void Show(TextBoxData data)
        {
            _autoPlay = data.AutoPlay;
            _autoPagePause = data.AutoPagePause;

            _hideTransition = data.HideTransition;

            _ui.SetText(data.Text);
            _ui.ShowBoard(data.ShowTransition);
            
            _typeRunner.SetSpeed(data.DefaultSpeed);
            _typeRunner.Run(_ui);
            _textChanger.ClearAll();
            _commandParser.Init(_ui.GetTextInfo());
            _voiceSpeaker.SetProfile(data.Voice);
            _voiceSpeaker.Resume();
            _input.Enable();
        }

        public void Hide()
        {
            StopAutoTurn();
            _typeRunner.Stop();
            _voiceSpeaker.Mute();
            _textChanger.ClearAll();
            _ui.HideBoard(_hideTransition);
            _input.Disable();
            _canTurnPage = false;
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
