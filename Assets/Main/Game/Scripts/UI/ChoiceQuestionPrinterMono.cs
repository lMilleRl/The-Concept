using System;
using System.Collections;
using System.Collections.Generic;
using TextBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Choices
{
    public class ChoiceQuestionPrinterMono : MonoBehaviour, IChoiceQuestionPrinter, ICoroutineRunner
    {
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TextBoxRegistry _textBoxRegistry;
        [SerializeField] private TextRegistry _textRegistry;
        [SerializeField] private EventRegistry _eventRegistry;
        [SerializeField] private ProgressiveTargetBase[] _progressiveTargets;
        [SerializeField] private AudioSource _voiceAudioSource;
        [SerializeField] private TextStyleProfile _defaultStyleProfile;

        private TypeRunner _typeRunner;
        private TextBoxUI _ui;
        private CommandParser _commandParser;
        private PrintOnlyTextBoxFacade _facade;
        private ITextBoxVoiceSpeaker _voiceSpeaker;
        private ProgressiveCommand _progressiveCommand;
        private MuteCommand _muteCommand;
        private bool _isInitialized;

        private void Awake()
        {
            if (_questionText == null || _textBoxRegistry == null || _textRegistry == null)
            {
                Debug.LogError($"[{nameof(ChoiceQuestionPrinterMono)}] Assign question text and TextBox registries.", this);
                return;
            }

            var debugWriter = new UnityDebugWriter();
            _typeRunner = new TypeRunner(this, debugWriter);
            _ui = new TextBoxUI(_questionText, new PrintOnlyTextBoxBoard(), TextOverflowModes.Overflow);

            var effects = TextBoxCompositionFactory.CreateEffects(
                _textBoxRegistry.Effects ?? Array.Empty<TextEffectEntry>(), _typeRunner);
            var textFormChanger = new TextFormChanger(effects, debugWriter);
            _voiceSpeaker = _voiceAudioSource != null
                ? new TextBoxVoiceSpeaker(_voiceAudioSource)
                : new NullTextBoxVoiceSpeaker();
            _typeRunner.OnCharPrinted += _voiceSpeaker.PlayChar;

            var progressiveTargets = new List<IProgressiveTarget>();
            if (_progressiveTargets != null)
            {
                for (int i = 0; i < _progressiveTargets.Length; i++)
                {
                    if (_progressiveTargets[i] != null)
                        progressiveTargets.Add(_progressiveTargets[i]);
                }
            }

            var progressiveTargetService = new ProgressiveTargetService(progressiveTargets);
            var commandEntries = _textBoxRegistry.Commands ?? Array.Empty<TextCommandEntry>();
            IEventRegistry eventRegistry = _eventRegistry != null ? _eventRegistry : new NullEventRegistry();
            var commands = TextBoxCompositionFactory.CreateCommands(commandEntries, _typeRunner, _voiceSpeaker,
                textFormChanger, progressiveTargetService, debugWriter, eventRegistry);
            var coordinator = new CommandCoordinator(commands);
            _commandParser = new CommandParser(coordinator, _typeRunner, new TagParser());
            _facade = new PrintOnlyTextBoxFacade(
                _ui, _typeRunner, _commandParser, _voiceSpeaker, textFormChanger, _defaultStyleProfile);

            TextBoxCompositionFactory.RegisterFacadeCommands(coordinator, commandEntries, _typeRunner,
                _facade, progressiveTargetService, _textRegistry, _voiceSpeaker, debugWriter,
                out _progressiveCommand, out _muteCommand);

            _isInitialized = true;
        }

        public IEnumerator Print(TextBoxData data)
        {
            if (!_isInitialized || data == null || string.IsNullOrWhiteSpace(data.Text))
                yield break;

            bool isFinished = false;
            Action onFinished = () => isFinished = true;
            _facade.OnHidden += onFinished;

            try
            {
                _facade.Show(data);
                yield return new WaitUntil(() => isFinished);
            }
            finally
            {
                _facade.OnHidden -= onFinished;
            }
        }

        public void Clear()
        {
            _facade?.Hide();
            if (_questionText != null)
                _questionText.text = string.Empty;
        }

        private void OnDestroy()
        {
            if (_typeRunner != null && _voiceSpeaker != null)
                _typeRunner.OnCharPrinted -= _voiceSpeaker.PlayChar;
            _progressiveCommand?.Dispose();
            _muteCommand?.Dispose();
            _commandParser?.Dispose();
            _facade?.Dispose();
            _typeRunner?.Dispose();
        }

        private sealed class PrintOnlyTextBoxBoard : ITextBoxBoard
        {
            public void Show(BoardTransitionContext context) { }
            public void Hide(BoardTransitionContext context) { }
        }

        private sealed class NullTextBoxVoiceSpeaker : ITextBoxVoiceSpeaker
        {
            public void SetProfile(VoiceProfile profile) { }
            public void PlayChar(char c) { }
            public void Mute() { }
            public void Resume() { }
        }

        private sealed class NullEventRegistry : IEventRegistry
        {
            public void Invoke(int eventId) => Debug.LogWarning($"[ChoiceQuestionPrinterMono] No EventRegistry assigned for text event {eventId}.");
            public UnityEvent GetEvent(int eventId) => null;
        }
    }
}