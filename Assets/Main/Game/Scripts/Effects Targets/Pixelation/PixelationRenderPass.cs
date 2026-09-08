using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Pixelation.Runtime
{
    internal sealed class PixelationRenderPass : ScriptableRenderPass, IDisposable
    {
        private const string RenderPassName = "Pixelation RenderPass";

        private static readonly int ProgressId = Shader.PropertyToID("_Progress");
        private static readonly int MaxPixelSizeId = Shader.PropertyToID("_MaxPixelSize");

        private readonly ProfilingSampler _profilingSampler = new(RenderPassName);
        private readonly Material _material;

        private RTHandle _mainFrame;

        public PixelationRenderPass(Shader shader, RenderPassEvent passEvent)
        {
            renderPassEvent = passEvent;
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        public void Dispose()
        {
            _mainFrame?.Release();
            CoreUtils.Destroy(_material);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref _mainFrame, descriptor, FilterMode.Bilinear,
                TextureWrapMode.Clamp, name: "_PixelationMainFrame");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var volume = VolumeManager.instance.stack.GetComponent<PixelationVolume>();
            if (_material == null || volume == null || !volume.IsActive ||
                !renderingData.cameraData.postProcessEnabled || renderingData.cameraData.isSceneViewCamera)
                return;

            var cmd = CommandBufferPool.Get(RenderPassName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                _material.SetFloat(ProgressId, volume.progress.value);
                _material.SetFloat(MaxPixelSizeId, volume.maxPixelSize.value);

                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                Blitter.BlitCameraTexture(cmd, source, _mainFrame);
                Blitter.BlitCameraTexture(cmd, _mainFrame, source, _material, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
