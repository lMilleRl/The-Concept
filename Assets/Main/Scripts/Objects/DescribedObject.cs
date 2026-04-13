using UnityEngine;

public class DescribedObject : MonoBehaviour, IInteractable
{
    [SerializeField] private TextForTextBox _textToShow;

    private TextBoxHandler _textShower;

    private void Start()
    {
        _textShower = FindObjectOfType<TextBoxHandler>();
    }

    public void Activate()
    {
        _textShower.ShowText(_textToShow);
    }
}