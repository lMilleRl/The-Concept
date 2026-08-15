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
        private readonly IDebugWriter _debugWriter;

        private ITextBoxUI _ui;
        private TMP_Text _text;
        private TMP_MeshInfo[] _cachedMeshInfo;
        private float _canvasScale = 1f;

        public TextFormChanger(ITextEffect[] effects, IDebugWriter debugWriter, ITextBoxUI _textMeshSource)
        {
            _effects = new Dictionary<TextBoxCommandType, ITextEffect>(effects.Length);
            _debugWriter = debugWriter;

            foreach (var effect in effects)
                _effects.TryAdd(effect.EffectType, effect);

            _textMeshSource.OnTextMeshUpdated += InitCashedMesh;
        }

        public void SetText(ITextBoxUI ui)
        {
            if (_ui != null)
            {
                _ui.ContentText.OnPreRenderText -= OnPreRenderText;
            }

            _ui = ui;
            _text = ui.ContentText;

            Canvas canvas = ui.Canvas;
            float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
            _canvasScale = scaleFactor > 0f ? 1f / scaleFactor : 1f;

            _text.OnPreRenderText += OnPreRenderText;
        }

        private void InitCashedMesh(TMP_TextInfo textInfo)
        {
            _cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
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
        }

        public void Tick()
        {
            if (_text == null || _activeEffects.Count == 0)
                return;

            _isFromTick = true;
            _text.ForceMeshUpdate();
            _isFromTick = false;
        }

        private bool _isFromTick;

        private void OnPreRenderText(TMP_TextInfo textInfo)
        {
            if (_cachedMeshInfo == null || !_isFromTick)
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
                    if (!charInfo.isVisible && i >= _text.maxVisibleCharacters) continue;

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

            _text.UpdateVertexData();
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