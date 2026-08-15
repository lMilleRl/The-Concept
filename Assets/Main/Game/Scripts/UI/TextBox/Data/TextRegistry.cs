using System.Collections.Generic;
using UnityEngine;

namespace TextBox
{
    [CreateAssetMenu(fileName = "TextRegistry", menuName = "TextBox/Text Registry")]
    public class TextRegistry : ScriptableObject, ITextRegistry
    {
        [SerializeField] private TextIdEntry[] _entries;

        private Dictionary<TextId, TextBoxData> _cache;

        public TextBoxData Get(TextId id)
        {
            if (_cache == null)
                BuildCache();

            _cache.TryGetValue(id, out var data);
            return data;
        }

        private void BuildCache()
        {
            _cache = new Dictionary<TextId, TextBoxData>();

            if (_entries == null)
                return;

            for (int i = 0; i < _entries.Length; i++)
            {
                TextId id = _entries[i].Id;

                if (id == TextId.None)
                {
                    Debug.LogWarning($"[TextRegistry] Entry at index {i} has {nameof(TextId.None)} and will be ignored.");
                    continue;
                }

                if (_cache.ContainsKey(id))
                    Debug.LogWarning($"[TextRegistry] Duplicate ID {id} at index {i}. Overwriting previous entry.");

                _cache[id] = _entries[i].Data;
            }
        }
    }
}
