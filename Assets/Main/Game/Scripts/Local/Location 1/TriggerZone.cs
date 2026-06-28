using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZone : MonoBehaviour
{
    [SerializeField] private string _targetTag = "Player";
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void OnEnable()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        if (other.CompareTag(_targetTag))
            OnEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) return;
        if (other.CompareTag(_targetTag))
            OnExit?.Invoke();
    }
}
