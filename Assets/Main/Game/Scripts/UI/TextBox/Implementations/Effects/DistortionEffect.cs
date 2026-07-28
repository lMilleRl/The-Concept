using UnityEngine;

namespace TextBox
{
    public class DistortionEffect : TextEffectBase
    {
        private const float DefaultAmplitude = 10f;
        private const float DefaultSpeed = 1f;
        private const float NoiseInputScale = 0.5f;
        private const float YNoiseOffset = 100f;
        private const float NoiseRemapScale = 2f;

        public DistortionEffect(EffectParams @params) : base(@params) { }

        public override TextBoxCommandType EffectType => TextBoxCommandType.Distortion;

        public override Vector3 Apply(int charIndex, Vector3 originalVertex, float[] effectParams)
        {
            float amplitude = GetParam(effectParams, 0, Params.Amplitude, DefaultAmplitude);
            float speed = GetParam(effectParams, 1, Params.Speed, DefaultSpeed);

            float time = Time.time * speed;
            float inputScale = Params.NoiseScale > 0f ? Params.NoiseScale : NoiseInputScale;

            float noiseX = Mathf.PerlinNoise(originalVertex.x * inputScale + time, originalVertex.y * inputScale) * NoiseRemapScale - 1f;
            float noiseY = Mathf.PerlinNoise(originalVertex.y * inputScale + YNoiseOffset + time, originalVertex.x * inputScale) * NoiseRemapScale - 1f;

            return originalVertex + new Vector3(noiseX, noiseY, 0f) * amplitude;
        }
    }
}
