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

        public ShakeEffect(EffectParams @params) : base(@params) { }

        public override TextBoxCommandType EffectType => TextBoxCommandType.Shake;

        public override Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams)
        {
            float amplitude = GetParam(effectParams, 0, Params.Amplitude, DefaultAmplitude);
            float speed = GetParam(effectParams, 1, Params.Speed, DefaultSpeed);

            float time = Time.time * speed;

            float x = Mathf.Sin((charIndex * XSeed + time) * TwoPi) * amplitude;
            float y = Mathf.Cos((charIndex * YSeed + time) * TwoPi) * amplitude;

            return originalVertex + new Vector3(x, y, 0f);
        }
    }
}
