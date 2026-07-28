using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TextChanger : ITextChanger
    {
        private readonly List<TextEffectData> _activeEffects = new List<TextEffectData>();
        private readonly Dictionary<TextBoxCommandType, ITextEffect> _effects;

        private TMP_Text _text;
        private TMP_MeshInfo[] _cachedMeshInfo;

        public TextChanger(ITextEffect[] effects)
        {
            _effects = new Dictionary<TextBoxCommandType, ITextEffect>(effects.Length);

            foreach (var effect in effects)
                _effects.TryAdd(effect.EffectType, effect);
        }

        public void SetText(TMP_Text text)
        {
            _text = text;
            _text.ForceMeshUpdate();
            _cachedMeshInfo = _text.textInfo.CopyMeshInfoVertexData();
        }

        public void AddEffect(TextEffectData effectData)
        {
            _activeEffects.Add(effectData);
        }

        public void RemoveEffect(TextEffectData effectData)
        {
            _activeEffects.Remove(effectData);
        }

        public void ClearAll()
        {
            _activeEffects.Clear();
            RestoreVertices();
        }

        public void Tick()
        {
            if (_text == null || _activeEffects.Count == 0)
                return;

            TMP_TextInfo textInfo = _text.textInfo;
            RestoreVertices();

            for (int e = 0; e < _activeEffects.Count; e++)
            {
                TextEffectData effectData = _activeEffects[e];

                if (!_effects.TryGetValue(effectData.EffectType, out ITextEffect effect))
                    continue;

                int end = effectData.StartCharIndex + effectData.CharLength;

                for (int i = effectData.StartCharIndex; i < end && i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] sourceVertices = _cachedMeshInfo[materialIndex].vertices;
                    Vector3[] destVertices = textInfo.meshInfo[materialIndex].vertices;

                    for (int v = 0; v < 4; v++)
                    {
                        Vector3 original = sourceVertices[vertexIndex + v];
                        destVertices[vertexIndex + v] = effect.Apply(i, original, effectData.Params);
                    }
                }
            }

            UpdateMesh(textInfo);
        }

        private void RestoreVertices()
        {
            if (_text == null || _cachedMeshInfo == null) return;

            TMP_TextInfo textInfo = _text.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var source = _cachedMeshInfo[i].vertices;
                var dest = textInfo.meshInfo[i].vertices;
                System.Array.Copy(source, dest, source.Length);
            }

            UpdateMesh(textInfo);
        }

        private void UpdateMesh(TMP_TextInfo textInfo)
        {
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
}
