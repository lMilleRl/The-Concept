using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGlitch.Runtime.TVShutdown
{
    sealed class TVShutdownRenderPass : ScriptableRenderPass, IDisposable
    {
        const string RenderPassName = "TVShutdown RenderPass";

        static readonly int MainTexID = Shader.PropertyToID("_MainTex");
        static readonly int ProgressID = Shader.PropertyToID("_Progress");
        static readonly int FlashIntensityID = Shader.PropertyToID("_FlashIntensity");
        static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");

        readonly ProfilingSampler _profilingSampler;
        readonly Material _material;
        readonly TVShutdownVolume _volume;

        RenderTargetHandle _mainFrame;

        bool isActive =>
            _material != null &&
            _volume != null &&
            _volume.IsActive;

        public TVShutdownRenderPass(Shader shader)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _profilingSampler = new ProfilingSampler(RenderPassName);
            _material = CoreUtils.CreateEngineMaterial(shader);

            var volumeStack = VolumeManager.instance.stack;
            _volume = volumeStack.GetComponent<TVShutdownVolume>();

            _mainFrame.Init("_MainFrame");
        }

        public void Dispose()
        {
            CoreUtils.Destroy(_material);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var isPostProcessEnabled = renderingData.cameraData.postProcessEnabled;
            var isSceneViewCamera = renderingData.cameraData.isSceneViewCamera;
            if (!isActive || !isPostProcessEnabled || isSceneViewCamera)
                return;

            var cmd = CommandBufferPool.Get(RenderPassName);
            cmd.Clear();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                var source = renderingData.cameraData.renderer.cameraColorTarget;

                var descriptor = renderingData.cameraData.cameraTargetDescriptor;
                descriptor.depthBufferBits = 0;
                cmd.GetTemporaryRT(_mainFrame.id, descriptor);
                cmd.Blit(source, _mainFrame.Identifier());

                _material.SetFloat(ProgressID, _volume.progress.value);
                _material.SetFloat(FlashIntensityID, _volume.flashIntensity.value);
                _material.SetColor(FlashColorID, _volume.flashColor.value);

                cmd.SetGlobalTexture(MainTexID, _mainFrame.Identifier());
                cmd.Blit(_mainFrame.Identifier(), source, _material);
                cmd.ReleaseTemporaryRT(_mainFrame.id);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
