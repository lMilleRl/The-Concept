using TMPro;
using UnityEngine;

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

        [Header("Progressive Targets")]
        [SerializeField] private ProgressiveTargetBase[] _progressiveTargets;

        [Header("Style")]
        [SerializeField] private TextStyleProfile _defaultStyleProfile;

        private void Awake()
        {
            var input = new TextBoxInput(_turnPageKey);
            _inputMono.Init(input);

            var boardTransitions = CreateTransitions(_registry.Transitions);
            _board.Init(boardTransitions);

            var textEffects = CreateEffects(_registry.Effects);
            var textChanger = new TextFormChanger(textEffects);
            _textChangerMono.Init(textChanger);

            var ui = new TextBoxUI(_contentText, _board);
            var voiceSpeaker = new TextBoxVoiceSpeaker(_voiceAudioSource);

            ICoroutineRunner coroutineRunner = _facadeMono;
            var typeRunner = new TypeRunner(coroutineRunner);

            var progressiveTargetService = new ProgressiveTargetService(_progressiveTargets);
            var commands = CreateCommands(_registry.Commands, typeRunner, voiceSpeaker, textChanger, progressiveTargetService);
            var coordinator = new CommandCoordinator(commands);
            var commandParser = new CommandParser(coordinator, typeRunner);

            var facadeData = new TextBoxFacadeData
            {
                UI = ui,
                TypeRunner = typeRunner,
                CommandParser = commandParser,
                VoiceSpeaker = voiceSpeaker,
                TextFormChanger = textChanger,
                Input = input,
                CoroutineRunner = coroutineRunner,
                DefaultStyle = _defaultStyleProfile
            };

            var facade = new TextBoxFacade(facadeData);
            _facadeMono.Init(facade);
        }

        private ITextEffect[] CreateEffects(TextEffectEntry[] entries)
        {
            var effects = new ITextEffect[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                effects[i] = entries[i].Type switch
                {
                    TextBoxCommandType.Wave => new WaveEffect(entries[i].Params),
                    TextBoxCommandType.Shake => new ShakeEffect(entries[i].Params),
                    TextBoxCommandType.Distortion => new DistortionEffect(entries[i].Params),
                    _ => null
                };
            }

            return effects;
        }

        private ITextBoxCommand[] CreateCommands(TextCommandEntry[] entries,
            ITypeRunner typeRunner, ITextBoxVoiceSpeaker voiceSpeaker, ITextFormChanger textFormChanger,
            IProgressiveTargetService progressiveTargetService)
        {
            var commands = new ITextBoxCommand[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                commands[i] = entries[i].Type switch
                {
                    TextBoxCommandType.Pause => new PauseCommand(typeRunner, entries[i].DefaultValue),
                    TextBoxCommandType.Speed => new SpeedCommand(typeRunner, entries[i].DefaultValue),
                    TextBoxCommandType.EaseSpeed => new EaseSpeedCommand(typeRunner, entries[i].DefaultValue),
                    TextBoxCommandType.Progressive => new ProgressiveCommand(typeRunner, progressiveTargetService, (ProgressiveTargetId)(int)entries[i].DefaultValue),
                    TextBoxCommandType.Wave or
                    TextBoxCommandType.Shake or
                    TextBoxCommandType.Distortion => new EffectCommand(entries[i].Type, textFormChanger),
                    _ => null
                };
            }

            return commands;
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
