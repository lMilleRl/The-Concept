Shader "Custom/SpriteEmission"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0.67,1,1)
        _EmissionIntensity ("Emission Intensity", Float) = 3.0
        _OutlineWidth ("Outline Width (px)", Range(0, 10)) = 2

        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

            #pragma multi_compile_instancing
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                half4   color           : COLOR;
                float2  uv              : TEXCOORD0;
                half2   lightingUV      : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS      : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            half4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            half4 _RendererColor;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _OutlineWidth;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            half GetOutlineAlpha(float2 uv)
            {
                float2 texel = _MainTex_TexelSize.xy * _OutlineWidth;
                half maxAlpha = 0;

                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, 0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, 0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, -texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, -texel.y)).a);

                return maxAlpha;
            }

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            #ifdef UNITY_INSTANCING_ENABLED
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteFlip);
            #endif
                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * _RendererColor;
            #ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
            #endif
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                const half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

                half4 result = CombinedShapeLightShared(surfaceData, inputData);

                half outlineAlpha = GetOutlineAlpha(i.uv);
                half outlineMask = saturate(outlineAlpha - main.a);

                half3 outlineColor = _EmissionColor.rgb * _EmissionIntensity;
                result.rgb = lerp(result.rgb, outlineColor, outlineMask);
                result.a = max(result.a, outlineMask);

                return result;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            half4 _RendererColor;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _OutlineWidth;

            half GetOutlineAlpha(float2 uv)
            {
                float2 texel = _MainTex_TexelSize.xy * _OutlineWidth;
                half maxAlpha = 0;

                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, 0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, 0)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x,  texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, -texel.y)).a);
                maxAlpha = max(maxAlpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, -texel.y)).a);

                return maxAlpha;
            }

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            #ifdef UNITY_INSTANCING_ENABLED
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteFlip);
            #endif
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
            #ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
            #endif
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                half outlineAlpha = GetOutlineAlpha(i.uv);
                half outlineMask = saturate(outlineAlpha - mainTex.a);

                float3 outlineColor = _EmissionColor.rgb * _EmissionIntensity;
                mainTex.rgb = lerp(mainTex.rgb, outlineColor, outlineMask);
                mainTex.a = max(mainTex.a, outlineMask);

                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
