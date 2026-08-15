using UnityEngine;

namespace TextBox
{
    public class WaveEffect : TextEffectBase
    {
        private const float DefaultAmplitude = 10f;
        private const float DefaultFrequency = 0.5f;
        private const float DefaultSpeed = 1f;
        private const float TwoPi = Mathf.PI * 2f;
        private const int BaseParamCount = 3;

        public WaveEffect(EffectParams @params, ICharProgressProvider progressProvider) : base(@params, progressProvider) { }

        public override TextBoxCommandType EffectType => TextBoxCommandType.Wave;

        public override Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams, int startCharIndex, int charLength)
        {
            var (baseParams, ease) = ExtractEase(effectParams, BaseParamCount);
            float progress = GetProgress(charIndex, startCharIndex, charLength, ease);

            float amplitude = GetParam(baseParams, 0, Params.Amplitude, DefaultAmplitude);
            float frequency = GetParam(baseParams, 1, Params.Frequency, DefaultFrequency);
            float speed = GetParam(baseParams, 2, Params.Speed, DefaultSpeed);

            float phase = (charIndex * frequency + Time.time * speed) * TwoPi;
            float yOffset = Mathf.Sin(phase) * amplitude * progress;

            return originalVertex + Vector3.up * yOffset;
        }
    }
}
