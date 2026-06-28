using System;
using UnityEngine;

public class TextTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TextForTextBox _textToShow;

    private void OnEnable()
    {
        
    }

    public void Activate()
    {
        TextBoxHandler.Instance.ShowText(_textToShow);
    }
}