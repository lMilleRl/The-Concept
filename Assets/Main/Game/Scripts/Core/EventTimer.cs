using UnityEngine;
using UnityEngine.Events;

public class EventTimer : MonoBehaviour
{
    [Range(0f, float.MaxValue)] [SerializeField] private float _delayInSec;
    public UnityEvent OnElapsed;

    public void StartTimer()
    {
        Invoke(nameof(Elapsed), _delayInSec);
    }

    public void CancelTimer()
    {
        CancelInvoke(nameof(Elapsed));
    }

    private void Elapsed()
    {
        OnElapsed?.Invoke();
    }
}
