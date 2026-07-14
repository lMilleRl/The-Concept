using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class VerticalDirectionalTrigger2D : MonoBehaviour
{
    [SerializeField] private string _targetTag = "Player";

    public UnityEvent OnExitFromBottomToTop;
    public UnityEvent OnExitFromTopToBottom;

    private Vector2? _lastEnterPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        if (!other.CompareTag(_targetTag)) return;

        _lastEnterPosition = other.transform.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) return;
        if (!other.CompareTag(_targetTag)) return;
        if (_lastEnterPosition == null) return;

        float deltaY = other.transform.position.y - _lastEnterPosition.Value.y;
        _lastEnterPosition = null;

        if (deltaY > 0)
        {
            OnExitFromBottomToTop?.Invoke();
        }
        else
        {
            OnExitFromTopToBottom?.Invoke();
        }
    }
}
