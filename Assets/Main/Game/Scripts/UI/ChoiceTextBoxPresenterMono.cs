using System;
using System.Collections;
using TextBox;
using UnityEngine;

namespace Choices
{
    public class ChoiceTextBoxPresenterMono : MonoBehaviour, IChoiceTextBoxPresenter
    {
        [SerializeField] private TextBoxFacadeMono _textBoxFacadeMono;

        public IEnumerator ShowAndWait(TextBoxData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Text))
                yield break;

            if (_textBoxFacadeMono == null || _textBoxFacadeMono.Facade == null)
            {
                Debug.LogError($"[{nameof(ChoiceTextBoxPresenterMono)}] TextBoxFacadeMono is not initialized.", this);
                yield break;
            }

            var facade = _textBoxFacadeMono.Facade;
            bool isHidden = false;
            Action onHidden = () => isHidden = true;
            facade.OnHidden += onHidden;

            try
            {
                _textBoxFacadeMono.Show(data);
                yield return new WaitUntil(() => isHidden);
            }
            finally
            {
                facade.OnHidden -= onHidden;
            }
        }
    }
}