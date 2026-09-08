using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Pixelation.Runtime
{
    public sealed class PixelationFeature : ScriptableRendererFeature
    {
        [SerializeField] private Shader _shader;
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        private PixelationRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new PixelationRenderPass(_shader, _renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var volume = VolumeManager.instance.stack.GetComponent<PixelationVolume>();
            if (volume != null && volume.IsActive && renderingData.cameraData.postProcessEnabled &&
                !renderingData.cameraData.isSceneViewCamera)
                renderer.EnqueuePass(_renderPass);
        }

        protected override void Dispose(bool disposing)
        {
            _renderPass?.Dispose();
        }
    }
}
