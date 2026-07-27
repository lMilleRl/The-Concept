using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TextBoxCompositionRoot : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _contentText;
        [SerializeField] private TextBoxBoard _board;

        [Header("Input")]
        [SerializeField] private KeyCode _turnPageKey = KeyCode.Space;

        [Header("Effects")]
        [SerializeField] private TextBoxFacadeMono _facadeMono;
        [SerializeField] private TextChangerMono _textChangerMono;
        [SerializeField] private TextBoxInputMono _inputMono;

        [Header("Audio")]
        [SerializeField] private AudioSource _voiceAudioSource;

        [Header("Transitions")]
        [SerializeField] private MonoBehaviour[] _boardTransitions;

        [Header("Text Effects")]
        [SerializeField] private MonoBehaviour[] _textEffects;

        [Header("Commands")]
        [SerializeField] private MonoBehaviour[] _textBoxCommands;

        private void Awake()
        {
            var input = new TextBoxInput(_turnPageKey);
            _inputMono.Init(input);

            var boardTransitions = GetInterfaces<IBoardTransition>(_boardTransitions);
            _board.Init(boardTransitions);

            var textEffects = GetInterfaces<ITextEffect>(_textEffects);
            var textChanger = new TextChanger(textEffects);
            textChanger.SetText(_contentText);
            _textChangerMono.Init(textChanger);

            var ui = new TextBoxUI(_contentText, _board);
            var voiceSpeaker = new TextBoxVoiceSpeaker(_voiceAudioSource);

            var commands = GetInterfaces<ITextBoxCommand>(_textBoxCommands);
            var coordinator = new CommandCoordinator(commands);

            ICoroutineRunner coroutineRunner = _facadeMono;

            var typeRunner = new TypeRunner(coroutineRunner);
            var commandParser = new CommandParser(coordinator, typeRunner);

            var facadeData = new TextBoxFacadeData
            {
                UI = ui,
                TypeRunner = typeRunner,
                CommandParser = commandParser,
                VoiceSpeaker = voiceSpeaker,
                TextChanger = textChanger,
                Input = input,
                CoroutineRunner = coroutineRunner
            };

            var facade = new TextBoxFacade(facadeData);
            _facadeMono.Init(facade);
        }

        private T[] GetInterfaces<T>(MonoBehaviour[] sources) where T : class
        {
            var result = new T[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                result[i] = sources[i] as T;

            return result;
        }
    }
}
