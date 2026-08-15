using UnityEngine;

namespace TextBox
{
    public class ShakeEffect : TextEffectBase
    {
        private const float DefaultAmplitude = 10f;
        private const float DefaultSpeed = 1f;
        private const float XSeed = 12.34f;
        private const float YSeed = 45.67f;
        private const float TwoPi = Mathf.PI * 2f;
        private const int BaseParamCount = 2;

        public ShakeEffect(EffectParams @params, ICharProgressProvider progressProvider) : base(@params, progressProvider) { }

        public override TextBoxCommandType EffectType => TextBoxCommandType.Shake;

        public override Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams, int startCharIndex, int charLength)
        {
            var (baseParams, ease) = ExtractEase(effectParams, BaseParamCount);
            float progress = GetProgress(charIndex, startCharIndex, charLength, ease);

            float amplitude = GetParam(baseParams, 0, Params.Amplitude, DefaultAmplitude);
            float speed = GetParam(baseParams, 1, Params.Speed, DefaultSpeed);
            float time = Time.time * speed;

            float x = Mathf.Sin((charIndex * XSeed + time) * TwoPi) * amplitude * progress;
            float y = Mathf.Cos((charIndex * YSeed + time) * TwoPi) * amplitude * progress;

            return originalVertex + new Vector3(x, y, 0f);
        }
    }
}
