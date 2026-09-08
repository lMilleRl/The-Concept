using System;
using UnityEngine.Rendering;

namespace Pixelation.Runtime
{
    [Serializable]
    [VolumeComponentMenu("Pixelation")]
    public sealed class PixelationVolume : VolumeComponent
    {
        public ClampedFloatParameter progress = new(0f, 0f, 1f);
        public ClampedFloatParameter maxPixelSize = new(32f, 1f, 256f);

        public bool IsActive => progress.value > 0f && maxPixelSize.value > 1f;
    }
}
