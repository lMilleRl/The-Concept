using UnityEngine;

public class CursorVisibilityController : MonoBehaviour
{
    [SerializeField] private bool _isVisible = true;

    private void OnEnable()
    {
        Cursor.visible = _isVisible;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            Cursor.visible = _isVisible;
    }
}