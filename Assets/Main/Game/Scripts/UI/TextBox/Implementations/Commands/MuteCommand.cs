using System;

namespace TextBox
{
    public class MuteCommand : ITextBoxCommand, IDisposable
    {
        private readonly ITypeRunner _typeRunner;
        private readonly ITextBoxVoiceSpeaker _voiceSpeaker;
        private readonly ITextBoxFacade _facade;

        private TextBoxCommandContext _activeContext;
        private bool _isActive;

        public TextBoxCommandType Type => TextBoxCommandType.Mute;

        public MuteCommand(ITypeRunner typeRunner, ITextBoxVoiceSpeaker voiceSpeaker, ITextBoxFacade facade)
        {
            _typeRunner = typeRunner;
            _voiceSpeaker = voiceSpeaker;
            _facade = facade;
        }

        public void Execute(TextBoxCommandContext context)
        {
            _activeContext = context;
            _isActive = true;

            _voiceSpeaker.Mute();

            _typeRunner.OnCharRevealed += HandleCharRevealed;
            _facade.OnCurrentTextEnded += HandleCurrentTextEnded;
        }

        private void HandleCharRevealed(int charIndex)
        {
            if (!_isActive)
                return;

            if (charIndex >= _activeContext.StartCharIndex + _activeContext.CharLength)
            {
                _voiceSpeaker.Resume();
                Unsubscribe();
            }
        }

        private void HandleCurrentTextEnded()
        {
            if (!_isActive)
                return;

            _voiceSpeaker.Resume();
            Unsubscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            _typeRunner.OnCharRevealed -= HandleCharRevealed;
            _facade.OnCurrentTextEnded -= HandleCurrentTextEnded;
            _isActive = false;
        }
    }
}
