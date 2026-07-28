using UnityEngine;

namespace TextBox
{
    public abstract class TextEffectBase : ITextEffect
    {
        private readonly EffectParams _params;

        public abstract TextBoxCommandType EffectType { get; }

        protected TextEffectBase(EffectParams @params)
        {
            _params = @params;
        }

        public abstract Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams);

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
