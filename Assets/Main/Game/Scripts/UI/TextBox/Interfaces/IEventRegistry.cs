using UnityEngine.Events;

namespace TextBox
{
    public interface IEventRegistry
    {
        void Invoke(int eventId);
        UnityEvent GetEvent(int eventId);
    }
}
