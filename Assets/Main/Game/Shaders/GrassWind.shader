Shader "TheConcept/GrassWind"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Grass properties)]
        _YWeight ("Y Weight's Offset", Float) = 1.0
        
        [Header(Wind Waves)]
        _Frequency1 ("Wave 1 Frequency", Float) = 1.0
        _Amplitude1 ("Wave 1 Amplitude", Float) = 0.05
        _Speed1 ("Wave 1 Speed", Float) = 1.0

        _Frequency2 ("Wave 2 Frequency", Float) = 2.3
        _Amplitude2 ("Wave 2 Amplitude", Float) = 0.02
        _Speed2 ("Wave 2 Speed", Float) = 1.7
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "GrassWindPass"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_INSTANCING_ENABLED
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                half2 lightingUV : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _Frequency1;
                float _Amplitude1;
                float _Speed1;
                float _Frequency2;
                float _Amplitude2;
                float _Speed2;
                float _YWeight;
            CBUFFER_END

            float2 _GlobalWindDirection;
            float _GlobalWindStrength;
            float _GlobalWindTime;
            
            float GetWavePhase(float2 worldPosition, float2 windDirection)
            {
                return dot(worldPosition, normalize(windDirection));
            }

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

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);

                float3 posOS = input.positionOS.xyz;

                // heightFactor: 0 at bottom (roots), 1 at top (tips)
                float heightFactor = input.uv.y;

                // World position for spatial variation
                float3 worldPos = TransformObjectToWorld(posOS);

                // Fourier series: 2 harmonics
                float phasePosition = GetWavePhase(worldPos.xy, _GlobalWindDirection);
                float wave1 = sin(phasePosition * _Frequency1 + _GlobalWindTime * _Speed1) * _Amplitude1;
                float wave2 = sin(phasePosition * _Frequency2 + _Time.y * _Speed2) * _Amplitude2;

                // Combined displacement
                float2 displacement = (wave1 + wave2) * heightFactor * _GlobalWindStrength * normalize(_GlobalWindDirection);

                posOS.x += displacement.x;
                posOS.y += displacement.y * _YWeight;

                output.positionCS = TransformObjectToHClip(posOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                output.lightingUV = half2(ComputeScreenPos(output.positionCS / output.positionCS.w).xy);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                const half4 main = input.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);

                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
