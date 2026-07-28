using UnityEngine;

namespace TextBox
{
    public class WaveEffect : TextEffectBase
    {
        private const float DefaultAmplitude = 10f;
        private const float DefaultFrequency = 0.5f;
        private const float DefaultSpeed = 1f;
        private const float TwoPi = Mathf.PI * 2f;

        public WaveEffect(EffectParams @params) : base(@params) { }

        public override TextBoxCommandType EffectType => TextBoxCommandType.Wave;

        public override Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams)
        {
            float amplitude = GetParam(effectParams, 0, Params.Amplitude, DefaultAmplitude);
            float frequency = GetParam(effectParams, 1, Params.Frequency, DefaultFrequency);
            float speed = GetParam(effectParams, 2, Params.Speed, DefaultSpeed);

            float phase = (charIndex * frequency + Time.time * speed) * TwoPi;
            float yOffset = Mathf.Sin(phase) * amplitude;

            return originalVertex + Vector3.up * yOffset;
        }
    }
}
