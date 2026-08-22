using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace URPGlitch.Runtime.TVShutdown
{
    public sealed class TVShutdownFeature : ScriptableRendererFeature
    {
        [SerializeField] private Shader shader;
        private TVShutdownRenderPass _scriptablePass;

        public override void Create()
        {
            _scriptablePass = new TVShutdownRenderPass(shader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_scriptablePass);
        }

        protected override void Dispose(bool disposing)
        {
            _scriptablePass.Dispose();
        }
    }
}
