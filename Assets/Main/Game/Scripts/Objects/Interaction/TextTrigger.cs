using TextBox;
using UnityEngine;

public class TextTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TextBoxData _textBoxData;

    private void OnEnable()
    {
        
    }

    public void Activate()
    {
        TextBoxFacadeMono.Instance.Show(_textBoxData);
    }
}