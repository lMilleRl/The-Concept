using System;
using System.Collections;
using TextBox;
using UnityEngine;

namespace Choices
{
    public class ChoiceCompositionRoot : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ChoiceMenuViewMono _view;
        [SerializeField] private ChoiceInputMono _input;

        [Header("TextBox adapters")]
        [SerializeField] private MonoBehaviour _textBoxPresenterSource;
        [SerializeField] private MonoBehaviour _questionPrinterSource;

        [Header("Audio")]
        [SerializeField] private AudioSource _choiceAudioSource;
        [SerializeField] private AudioClip _moveSelectionSfx;
        [SerializeField] private AudioClip _confirmSelectionSfx;

        private IChoiceTextBoxPresenter _textBoxPresenter;
        private IChoiceQuestionPrinter _questionPrinter;
        private ChoiceNavigationController _navigation;
        private ChoiceSequenceData _activeData;
        private ChoiceOptionEventBinding[] _activeEventBindings;
        private Coroutine _activeFlow;
        private GameState _previousGameState;
        private bool _hasGameStateSnapshot;
        private bool _isOpen;
        private bool _isSelectionResolved;
        private bool _wasCancelled;

        private void Awake()
        {
            _textBoxPresenter = _textBoxPresenterSource as IChoiceTextBoxPresenter;
            _questionPrinter = _questionPrinterSource as IChoiceQuestionPrinter;
            if (_textBoxPresenterSource != null && _textBoxPresenter == null)
                Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] TextBox presenter does not implement {nameof(IChoiceTextBoxPresenter)}.", this);
            if (_questionPrinterSource != null && _questionPrinter == null)
                Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Question printer does not implement {nameof(IChoiceQuestionPrinter)}.", this);
            _input?.Init(this);
            _input?.SetEnabled(false);
            _view?.Hide();
        }

        public void OpenChoice(ChoiceSequenceData data, ChoiceOptionEventBinding[] eventBindings)
        {
            if (_isOpen)
                return;

            if (data == null || data.Options.Length == 0)
            {
                Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Choice data must contain at least one option.", this);
                return;
            }

            if (_view == null || _input == null)
            {
                Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Assign a choice view and input wrapper.", this);
                return;
            }

            var options = data.Options;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == null)
                {
                    Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Choice option at index {i} is null.", this);
                    return;
                }

                for (int j = 0; j < i; j++)
                {
                    if (options[i].Id == options[j].Id)
                    {
                        Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Duplicate option ID {options[i].Id}.", this);
                        return;
                    }

                }
            }

            var bindings = eventBindings ?? Array.Empty<ChoiceOptionEventBinding>();
            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i] == null)
                    continue;

                for (int j = 0; j < i; j++)
                {
                    if (bindings[j] != null && bindings[i].OptionId == bindings[j].OptionId)
                    {
                        Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] Duplicate event binding for option ID {bindings[i].OptionId}.", this);
                        return;
                    }
                }
            }

            _isOpen = true;
            _activeData = data;
            _activeEventBindings = bindings;
            _activeFlow = StartCoroutine(RunChoice());
            if (!_isOpen)
                _activeFlow = null;
        }

        public void MoveSelection(Vector2Int direction)
        {
            if (!_isOpen || _isSelectionResolved || _navigation == null)
                return;

            if (_navigation.Move(direction))
            {
                _view.SetSelectedIndex(_navigation.SelectedIndex);
                PlaySound(_moveSelectionSfx);
            }
        }

        public void SubmitSelection()
        {
            if (!_isOpen || _isSelectionResolved || _navigation == null)
                return;

            _isSelectionResolved = true;
            PlaySound(_confirmSelectionSfx);
        }

        public void CancelSelection()
        {
            if (!_isOpen || _isSelectionResolved)
                return;

            _wasCancelled = true;
            _isSelectionResolved = true;
        }

        private void PlaySound(AudioClip clip)
        {
            if (_choiceAudioSource != null && clip != null)
                _choiceAudioSource.PlayOneShot(clip);
        }

        private IEnumerator RunChoice()
        {
            var gameStateManager = GameStateManager.Instance;
            if (gameStateManager != null)
            {
                _previousGameState = gameStateManager.CurrentState;
                _hasGameStateSnapshot = true;
                gameStateManager.SetState(GameState.PassiveShow);
            }

            if (HasText(_activeData.BeforeChoiceText))
                yield return ShowTextBoxMessage(_activeData.BeforeChoiceText);

            _navigation = new ChoiceNavigationController(_activeData.Options, _activeData.GridColumns);
            if (!_view.Show(_activeData, _navigation.SelectedIndex))
            {
                RestoreGameState();
                FinishChoice();
                yield break;
            }

            _isSelectionResolved = false;
            _wasCancelled = false;

            if (_activeData.ShowQuestion && HasText(_activeData.QuestionText))
            {
                if (_questionPrinter != null)
                {
                    var printQuestion = _questionPrinter.Print(_activeData.QuestionText);
                    if (printQuestion != null)
                        yield return printQuestion;
                }
                else if (_view.QuestionText != null)
                {
                    Debug.LogWarning($"[{nameof(ChoiceCompositionRoot)}] No print-only question printer assigned; showing the question without typing.", this);
                    _view.QuestionText.text = _activeData.QuestionText.Text;
                }
            }

            _input.SetEnabled(true);
            yield return new WaitUntil(() => _isSelectionResolved);
            _input.SetEnabled(false);
            _view.Hide();
            _questionPrinter?.Clear();

            if (_wasCancelled)
            {
                RestoreGameState();
                FinishChoice();
                yield break;
            }

            var selectedOption = _navigation.SelectedOption;
            var selectedEvent = FindEventBinding(selectedOption.Id);
            bool hasAfterText = HasText(selectedOption.AfterChoiceText);
            GameState previousGameState = _previousGameState;
            bool hasPreviousGameState = _hasGameStateSnapshot;

            if (selectedOption.ActionOrder == ChoiceOptionActionOrder.TextThenEvent)
            {
                if (hasAfterText)
                    yield return ShowTextBoxMessage(selectedOption.AfterChoiceText);

                RestoreGameState();
                selectedEvent?.Invoke();
            }
            else
            {
                RestoreGameState();
                selectedEvent?.Invoke();

                if (hasAfterText)
                {
                    yield return ShowTextBoxMessage(selectedOption.AfterChoiceText);
                    if (hasPreviousGameState && GameStateManager.Instance != null)
                        GameStateManager.Instance.SetState(previousGameState);
                }
            }

            FinishChoice();
        }

        private IEnumerator ShowTextBoxMessage(TextBoxData data)
        {
            if (_textBoxPresenter == null)
            {
                Debug.LogError($"[{nameof(ChoiceCompositionRoot)}] TextBox data is configured but no IChoiceTextBoxPresenter is assigned.", this);
                yield break;
            }

            var showText = _textBoxPresenter.ShowAndWait(data);
            if (showText != null)
                yield return showText;
        }

        private ChoiceOptionEventBinding FindEventBinding(int optionId)
        {
            for (int i = 0; i < _activeEventBindings.Length; i++)
            {
                if (_activeEventBindings[i] != null && _activeEventBindings[i].OptionId == optionId)
                    return _activeEventBindings[i];
            }

            return null;
        }

        private static bool HasText(TextBoxData data)
        {
            return data != null && !string.IsNullOrWhiteSpace(data.Text);
        }

        private void RestoreGameState()
        {
            if (!_hasGameStateSnapshot)
                return;

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.SetState(_previousGameState);

            _hasGameStateSnapshot = false;
        }

        private void FinishChoice()
        {
            _input?.SetEnabled(false);
            _view?.Hide();
            _questionPrinter?.Clear();
            _navigation = null;
            _activeData = null;
            _activeEventBindings = null;
            _activeFlow = null;
            _isSelectionResolved = false;
            _wasCancelled = false;
            _isOpen = false;
        }

        private void OnDisable()
        {
            if (!_isOpen)
                return;

            if (_activeFlow != null)
                StopCoroutine(_activeFlow);

            RestoreGameState();
            FinishChoice();
        }
    }
}