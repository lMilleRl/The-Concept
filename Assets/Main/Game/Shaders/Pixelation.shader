Shader "Hidden/Pixelation"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _BlitTexture_TexelSize;
            float _Progress;
            float _MaxPixelSize;

            half4 Fragment(Varyings input) : SV_Target
            {
                float pixelSize = max(1.0, round(lerp(1.0, _MaxPixelSize, saturate(_Progress))));
                float2 resolution = _BlitTexture_TexelSize.zw;
                float2 blockCount = max(floor(resolution / pixelSize), 1.0);
                float2 pixelatedUv = (floor(input.texcoord * blockCount) + 0.5) / blockCount;
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, pixelatedUv);
            }
            ENDHLSL
        }
    }
}
