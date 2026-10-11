using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

namespace TextBox
{
    public class TextBoxCompositionRoot : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _contentText;
        [SerializeField] private TextBoxBoard _board;

        [Header("Mono Wrappers")]
        [SerializeField] private TextBoxFacadeMono _facadeMono;
        [SerializeField] private TextChangerMono _textChangerMono;
        [SerializeField] private TextBoxInputMono _inputMono;

        [Header("Input")]
        [SerializeField] private KeyCode _turnPageKey = KeyCode.Z;

        [Header("Audio")]
        [SerializeField] private AudioSource _voiceAudioSource;

        [Header("Registry")]
        [SerializeField] private TextBoxRegistry _registry;
        [SerializeField] private TextRegistry _textRegistry;

        [Header("Progressive Targets")]
        [SerializeField] private ProgressiveTargetBase[] _progressiveTargets;

        [Header("Events")]
        [SerializeField] private EventRegistry _eventRegistry;

        [Header("Style")]
        [SerializeField] private TextStyleProfile _defaultStyleProfile;

        private TypeRunner _typeRunner;
        private CommandParser _commandParser;
        private TextBoxFacade _facade;
        private ProgressiveCommand _progressiveCommand;
        private MuteCommand _muteCommand;
        private TextBoxVoiceSpeaker _voiceSpeaker;

        private void Awake()
        {
            IDebugWriter debugWriter = new UnityDebugWriter();

            var input = new TextBoxInput(_turnPageKey);
            _inputMono.Init(input);

            var boardTransitions = CreateTransitions(_registry.Transitions);
            _board.Init(boardTransitions);

            ICoroutineRunner coroutineRunner = _facadeMono;
            _typeRunner = new TypeRunner(coroutineRunner, debugWriter);

            var textEffects = TextBoxCompositionFactory.CreateEffects(_registry.Effects, _typeRunner);

            var ui = new TextBoxUI(_contentText, _board);
            
            var textFormChanger = new TextFormChanger(textEffects, debugWriter);
            _textChangerMono.Init(textFormChanger);
            
            var voiceSpeaker = new TextBoxVoiceSpeaker(_voiceAudioSource);
            _voiceSpeaker = voiceSpeaker;
            _typeRunner.OnCharPrinted += voiceSpeaker.OnCharPrinted;

            var progressiveTargetService = new ProgressiveTargetService(_progressiveTargets);
            var eventRegistry = (IEventRegistry)_eventRegistry;
            var commands = TextBoxCompositionFactory.CreateCommands(_registry.Commands, _typeRunner, voiceSpeaker, textFormChanger, progressiveTargetService, debugWriter, eventRegistry);
            var coordinator = new CommandCoordinator(commands);
            _commandParser = new CommandParser(coordinator, _typeRunner, new TagParser());

            var facadeData = new TextBoxFacadeData
            {
                UI = ui,
                TypeRunner = _typeRunner,
                CommandParser = _commandParser,
                VoiceSpeaker = voiceSpeaker,
                TextFormChanger = textFormChanger,
                Input = input,
                CoroutineRunner = coroutineRunner,
                DefaultStyle = _defaultStyleProfile
            };

            _facade = new TextBoxFacade(facadeData);

            RegisterFacadeCommands(coordinator, _typeRunner, _facade, progressiveTargetService, debugWriter);

            _facadeMono.Init(_facade);
        }

        private void OnDestroy()
        {
            if (_typeRunner != null && _voiceSpeaker != null)
                _typeRunner.OnCharPrinted -= _voiceSpeaker.OnCharPrinted;
            _progressiveCommand?.Dispose();
            _muteCommand?.Dispose();
            _commandParser?.Dispose();
            _facade?.Dispose();
            _typeRunner?.Dispose();
        }

        private void RegisterFacadeCommands(ICommandCoordinator coordinator, ITypeRunner typeRunner,
            ITextBoxFacade facade, IProgressiveTargetService progressiveTargetService, IDebugWriter debugWriter)
        {
            TextBoxCompositionFactory.RegisterFacadeCommands(coordinator, _registry.Commands, typeRunner,
                facade, progressiveTargetService, _textRegistry, _voiceSpeaker, debugWriter,
                out _progressiveCommand, out _muteCommand);
        }

        private IBoardTransition[] CreateTransitions(BoardTransitionEntry[] entries)
        {
            var transitions = new IBoardTransition[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                // TODO: switch по entries[i].Type → new FadeTransition() и т.д.
                transitions[i] = null;
            }

            return transitions;
        }
    }
}
