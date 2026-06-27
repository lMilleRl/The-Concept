using UnityEngine;

public class TextTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TextForTextBox _textToShow;

    public void Activate()
    {
        TextBoxHandler.Instance.ShowText(_textToShow);
    }
}