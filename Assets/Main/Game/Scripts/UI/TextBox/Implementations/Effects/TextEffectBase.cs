using System;
using UnityEngine;

namespace TextBox
{
    public abstract class TextEffectBase : ITextEffect
    {
        private readonly EffectParams _params;
        private readonly ICharProgressProvider _progressProvider;

        public abstract TextBoxCommandType EffectType { get; }

        protected TextEffectBase(EffectParams @params, ICharProgressProvider progressProvider)
        {
            _params = @params;
            _progressProvider = progressProvider;
        }

        public abstract Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams, int startCharIndex, int charLength);

        protected (float[] baseParams, EaseType ease) ExtractEase(float[] allParams, int baseParamCount)
        {
            if (allParams.Length <= baseParamCount)
                return (allParams, EaseType.None);

            var baseParams = new float[baseParamCount];
            Array.Copy(allParams, baseParams, baseParamCount);
            var ease = (EaseType)(int)allParams[^1];
            return (baseParams, ease);
        }

        protected float GetProgress(int charIndex, int startCharIndex, int charLength, EaseType ease)
        {
            return _progressProvider.GetProgress(charIndex, startCharIndex, charLength, ease);
        }

        protected float GetParam(float[] runtime, int index, float configured, float fallback)
        {
            if (runtime.Length > index)
                return runtime[index];

            return configured > 0f ? configured : fallback;
        }

        protected float GetParam(float[] runtime, int index, float fallback)
        {
            return GetParam(runtime, index, 0f, fallback);
        }

        protected EffectParams Params => _params;
    }
}
