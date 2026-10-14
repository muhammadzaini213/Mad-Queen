
Shader "Custom/SpriteOutlineURP"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        [IntRange] _OutlineWidth ("Outline Width (pixels)", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Harus berada di luar UnityPerMaterial.
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 spriteColor =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        IN.uv
                    ) * IN.color;

                // Jarak sampling berdasarkan ukuran satu piksel tekstur.
                float2 offset =
                    abs(_MainTex_TexelSize.xy) * _OutlineWidth;

                // Cari alpha dari 8 arah di sekitar piksel.
                half surroundingAlpha = 0;

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(offset.x, 0)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(-offset.x, 0)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(0, offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(0, -offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(offset.x, offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(-offset.x, offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(offset.x, -offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex, sampler_MainTex,
                        IN.uv + float2(-offset.x, -offset.y)
                    ).a
                );

                // Outline hanya muncul di luar bentuk sprite.
                half outlineAlpha = saturate(
                    surroundingAlpha - spriteColor.a
                );

                half3 finalColor = lerp(
                    _OutlineColor.rgb,
                    spriteColor.rgb,
                    spriteColor.a
                );

                half finalAlpha = max(
                    spriteColor.a,
                    outlineAlpha * _OutlineColor.a
                );

                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }

        // Fallback untuk renderer yang tidak mendukung pass 2D di atas.
        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, IN.uv
                ) * IN.color;

                float2 d = abs(_MainTex_TexelSize.xy) * _OutlineWidth;
                half a = c.a;

                a = max(a, SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex,
                    IN.uv + float2(d.x, 0)
                ).a);

                a = max(a, SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex,
                    IN.uv + float2(-d.x, 0)
                ).a);

                a = max(a, SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex,
                    IN.uv + float2(0, d.y)
                ).a);

                a = max(a, SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex,
                    IN.uv + float2(0, -d.y)
                ).a);

                half ring = saturate(a - c.a);
                half3 rgb = lerp(_OutlineColor.rgb, c.rgb, c.a);
                half alpha = max(c.a, ring * _OutlineColor.a);

                return half4(rgb, alpha);
            }

            ENDHLSL
        }
    }
}
