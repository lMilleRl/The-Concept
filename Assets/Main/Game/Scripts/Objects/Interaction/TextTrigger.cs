using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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