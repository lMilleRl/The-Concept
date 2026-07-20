Shader "TheConcept/GrassWind"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Wind Waves)]
        _Frequency1 ("Wave 1 Frequency", Float) = 1.0
        _Amplitude1 ("Wave 1 Amplitude", Float) = 0.05
        _Speed1 ("Wave 1 Speed", Float) = 1.0

        _Frequency2 ("Wave 2 Frequency", Float) = 2.3
        _Amplitude2 ("Wave 2 Amplitude", Float) = 0.02
        _Speed2 ("Wave 2 Speed", Float) = 1.7

        [Header(Wind Global)]
        _WindDirection ("Wind Direction X", Float) = 1.0
        _WindStrength ("Wind Strength", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "GrassWindPass"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_INSTANCING_ENABLED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _Frequency1;
                float _Amplitude1;
                float _Speed1;
                float _Frequency2;
                float _Amplitude2;
                float _Speed2;
                float _WindDirection;
                float _WindStrength;
            CBUFFER_END

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
                float wave1 = sin(worldPos.x * _Frequency1 + _Time.y * _Speed1) * _Amplitude1;
                float wave2 = sin(worldPos.x * _Frequency2 + _Time.y * _Speed2) * _Amplitude2;

                // Combined displacement
                float displacement = (wave1 + wave2) * heightFactor * _WindStrength * _WindDirection;

                posOS.x += displacement;

                output.positionCS = TransformObjectToHClip(posOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                return tex * input.color;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
