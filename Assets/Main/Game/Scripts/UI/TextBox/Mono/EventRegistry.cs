using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TextBox
{
    [AddComponentMenu("TextBox/Event Registry")]
    public class EventRegistry : MonoBehaviour, IEventRegistry
    {
        [SerializeField] private EventEntry[] _entries;

        private Dictionary<int, UnityEvent> _cache;

        public void Invoke(int eventId)
        {
            if (_cache == null)
                BuildCache();

            if (_cache.TryGetValue(eventId, out var evt))
                evt.Invoke();
            else
                Debug.LogWarning($"[EventRegistry] No event found for ID {eventId}.");
        }

        public UnityEvent GetEvent(int eventId)
        {
            if (_cache == null)
                BuildCache();

            _cache.TryGetValue(eventId, out var evt);
            return evt;
        }

        private void BuildCache()
        {
            _cache = new Dictionary<int, UnityEvent>();

            if (_entries == null)
                return;

            for (int i = 0; i < _entries.Length; i++)
            {
                int id = _entries[i].Id;

                if (_cache.ContainsKey(id))
                    Debug.LogWarning($"[EventRegistry] Duplicate ID {id} at index {i}. Overwriting previous entry.");

                _cache[id] = _entries[i].Event;
            }
        }

        [System.Serializable]
        public struct EventEntry
        {
            public int Id;
            public UnityEvent Event;
        }
    }
}
