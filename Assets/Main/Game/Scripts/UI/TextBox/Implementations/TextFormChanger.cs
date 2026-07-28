using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TextFormChanger : ITextChanger
    {
        private readonly List<TextEffectData> _activeEffects = new List<TextEffectData>();
        private readonly Dictionary<TextBoxCommandType, ITextEffect> _effects;

        private TMP_Text _text;
        private TMP_MeshInfo[] _cachedMeshInfo;
        private float _canvasScale = 1f;

        public TextFormChanger(ITextEffect[] effects)
        {
            _effects = new Dictionary<TextBoxCommandType, ITextEffect>(effects.Length);

            foreach (var effect in effects)
                _effects.TryAdd(effect.EffectType, effect);
        }

        public void SetText(TMP_Text text)
        {
            if (_text != null)
                _text.OnPreRenderText -= OnPreRenderText;

            _text = text;

            Canvas canvas = _text.GetComponentInParent<Canvas>();
            float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
            _canvasScale = scaleFactor > 0f ? 1f / scaleFactor : 1f;

            _text.ForceMeshUpdate();
            _cachedMeshInfo = _text.textInfo.CopyMeshInfoVertexData();
            _text.OnPreRenderText += OnPreRenderText;
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
            _text?.ForceMeshUpdate(false, false);
        }

        public void Tick()
        {
            if (_text == null || _activeEffects.Count == 0)
                return;

            _text.ForceMeshUpdate(false, false);
        }

        private void OnPreRenderText(TMP_TextInfo textInfo)
        {
            if (_cachedMeshInfo == null)
                return;

            RestoreVertices(textInfo);

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
                        Vector3 modified = effect.Apply(i, original, effectData.Params);
                        Vector3 offset = modified - original;
                        destVertices[vertexIndex + v] = original + offset * _canvasScale;
                    }
                }
            }
        }

        private void RestoreVertices(TMP_TextInfo textInfo)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] source = _cachedMeshInfo[materialIndex].vertices;
                Vector3[] dest = textInfo.meshInfo[materialIndex].vertices;

                for (int v = 0; v < 4; v++)
                    dest[vertexIndex + v] = source[vertexIndex + v];
            }
        }
    }
}
