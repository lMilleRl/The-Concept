using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TextBox
{
    public class TextFormChanger : ITextFormChanger
    {
        private readonly List<TextEffectData> _activeEffects = new List<TextEffectData>();
        private readonly Dictionary<TextBoxCommandType, ITextEffect> _effects;

        private ITextBoxUI _ui;
        private TMP_Text _text;
        private TMP_MeshInfo[] _cachedMeshInfo;
        private float _canvasScale = 1f;

        public TextFormChanger(ITextEffect[] effects)
        {
            _effects = new Dictionary<TextBoxCommandType, ITextEffect>(effects.Length);

            foreach (var effect in effects)
                _effects.TryAdd(effect.EffectType, effect);
        }

        public void SetText(ITextBoxUI ui)
        {
            if (_ui != null)
            {
                _ui.ContentText.OnPreRenderText -= OnPreRenderText;
            }

            _ui = ui;
            _text = ui.ContentText;

            Canvas canvas = _text.GetComponentInParent<Canvas>();
            float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
            _canvasScale = scaleFactor > 0f ? 1f / scaleFactor : 1f;

            _text.OnPreRenderText += OnPreRenderText;
            InitCashedMesh();
        }

        private void InitCashedMesh()
        {
            int savedMaxVisible = _text.maxVisibleCharacters;
            _text.maxVisibleCharacters = _text.textInfo.characterCount;
            _text.ForceMeshUpdate();
            _cachedMeshInfo = _text.textInfo.CopyMeshInfoVertexData();
            _text.maxVisibleCharacters = savedMaxVisible;
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

            _isMeshFromTick = true;
            _text.ForceMeshUpdate(false, false);
            _isMeshFromTick = false;
        }

        private bool _isMeshFromTick;

        private void OnPreRenderText(TMP_TextInfo textInfo)
        {
            if (_cachedMeshInfo == null || !_isMeshFromTick)
                return;

            RestoreVertices(textInfo);

            foreach (var effectData in _activeEffects)
            {
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
            int maxVisible = _text.maxVisibleCharacters;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (i >= maxVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                if (materialIndex >= _cachedMeshInfo.Length)
                    continue;

                int vertexIndex = charInfo.vertexIndex;
                Vector3[] source = _cachedMeshInfo[materialIndex].vertices;
                Vector3[] dest = textInfo.meshInfo[materialIndex].vertices;

                if (source == null || dest == null || vertexIndex + 3 >= source.Length)
                    continue;

                dest[vertexIndex] = source[vertexIndex];
                dest[vertexIndex + 1] = source[vertexIndex + 1];
                dest[vertexIndex + 2] = source[vertexIndex + 2];
                dest[vertexIndex + 3] = source[vertexIndex + 3];
            }
        }
    }
}