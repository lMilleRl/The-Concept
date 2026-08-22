using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace URPGlitch.Runtime.TVShutdown
{
    [Serializable]
    [VolumeComponentMenu("TV Shutdown")]
    public class TVShutdownVolume : VolumeComponent
    {
        public ClampedFloatParameter progress = new(0f, 0f, 1f);
        public ClampedFloatParameter flashIntensity = new(0.8f, 0f, 1f);
        public ColorParameter flashColor = new(Color.white);

        public bool IsActive => progress.value > 0f;
    }
}
