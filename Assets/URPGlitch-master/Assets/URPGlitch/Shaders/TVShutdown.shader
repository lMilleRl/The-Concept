// TV Shutdown effect — old CRT power-off.
// Stages: vertical squash to centre line → horizontal collapse to point → white flash → fade.
// progress: 0 = fully visible, 1 = fully collapsed.
Shader "URPGlitch/RenderFeature/TVShutdown"
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
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                half4 positionCS : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Progress;
            float _FlashIntensity;
            half4 _FlashColor;

            // Stage timing thresholds
            static const float TB_TIME = 0.7;    // top-bottom collapse: 0 → 0.7
            static const float LR_DELAY = 0.6;   // left-right starts at 0.6
            static const float LR_TIME = 0.4;    // left-right: 0.6 → 1.0
            static const float FF_TIME = 0.1;    // final fade: last 10%
            static const float BLUR_WIDTH = 0.05;

            float easeOutQuad(float t)
            {
                return 1.0 - (1.0 - t) * (1.0 - t);
            }

            Varyings Vertex(Attributes i)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                output.uv = i.uv;
                return output;
            }

            half4 Fragment(Varyings i) : SV_Target
            {
                float prog = easeOutQuad(saturate(_Progress));

                // Vertical squash: content scales toward centre line at uv.y = 0.5
                float scale = 1.0 / lerp(1.0, 20.0, prog) - 1.0;
                float2 coords = i.uv;
                coords.y = scale * (coords.y - 0.5) + coords.y;

                // Per-stage progress
                float tbProg = smoothstep(0.0, 1.0, saturate(prog / TB_TIME));
                float lrProg = smoothstep(0.0, 1.0, saturate((prog - LR_DELAY) / LR_TIME));
                float ffProg = smoothstep(0.0, 1.0, saturate((prog - 1.0 + FF_TIME) / FF_TIME));

                // Top-centre-bottom gradient: 0 at edges, 1 at middle.
                // Uses original uv.y (not squashed coords.y) so the mask
                // collapses from top/bottom toward centre as tbProg increases.
                float tb = i.uv.y * 2.0;
                tb = tb < 1.0 ? tb : 2.0 - tb;

                // Left-centre-right gradient: 0 at edges, 1 at middle
                float lr = i.uv.x * 2.0;
                lr = lr < 1.0 ? lr : 2.0 - lr;

                // Masks
                float tbMask = 1.0 - smoothstep(0.0, 1.0, saturate((tbProg - tb) / BLUR_WIDTH));
                float lrMask = 1.0 - smoothstep(0.0, 1.0, saturate((lrProg - lr) / BLUR_WIDTH));
                float ffMask = 1.0 - smoothstep(0.0, 1.0, ffProg);
                float mask = tbMask * lrMask * ffMask;

                // Sample with squashed coords; force off-screen to transparent
                float2 inside = step(float2(0.0, 0.0), coords) * step(coords, float2(1.0, 1.0));
                float onScreen = inside.x * inside.y;
                half4 sampled = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coords) * onScreen;

                // Flash tint as collapse progresses
                float tintMix = _FlashIntensity * smoothstep(0.0, 1.0, prog);
                sampled.rgb = lerp(sampled.rgb, _FlashColor.rgb * sampled.a, tintMix);

                return sampled * mask;
            }
            ENDHLSL
        }
    }
}
