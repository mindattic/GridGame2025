Shader "Universal Render Pipeline/UI/DoubleOutlineImage"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor1("Outer Outline Color", Color) = (0,0,0,1) // Black
        _OutlineColor2("Inner Outline Color", Color) = (1,1,1,1) // White
        _OutlineSize1("Outer Outline Size", Float) = 3.0
        _OutlineSize2("Inner Outline Size", Float) = 1.5
    }

        SubShader
        {
            Tags {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
            }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                Name "DoubleOutlineUI"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Declare the texture. Do not declare a sampler explicitly.
            Texture2D _MainTex;

            float4 _MainTex_ST;
            float4 _OutlineColor1;
            float4 _OutlineColor2;
            float  _OutlineSize1;
            float  _OutlineSize2;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord   : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = mul(UNITY_MATRIX_MVP, IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                return OUT;
            }

            float4 SampleOutline(float2 uv, float size, float4 color)
            {
                float alpha = 0.0;
                float2 offsets[8] = {
                    float2(-1,  0), float2(1,  0),
                    float2(0, -1), float2(0,  1),
                    float2(-1, -1), float2(-1,  1),
                    float2(1, -1), float2(1,  1)
                };

                // Use the auto-generated sampler "sampler_MainTex" for _MainTex.
                for (int i = 0; i < 8; i++)
                {
                    float2 offsetUV = uv + offsets[i] * size / _ScreenParams.xy;
                    float4 sampleColor = _MainTex.Sample(sampler_MainTex, offsetUV);
                    alpha = max(alpha, sampleColor.a);
                }
                return color * alpha;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 original = _MainTex.Sample(sampler_MainTex, IN.uv);

                // Discard fully transparent pixels.
                clip(original.a - 0.01);

                // Outer (black) outline – larger.
                float4 outer = SampleOutline(IN.uv, _OutlineSize1, _OutlineColor1);
                // Inner (white) outline – smaller.
                float4 inner = SampleOutline(IN.uv, _OutlineSize2, _OutlineColor2);

                // Layer inner outline on top of outer outline.
                float4 layered = outer;
                layered.rgb = lerp(layered.rgb, inner.rgb, inner.a);
                layered.a = max(layered.a, inner.a);

                // Blend the layered outline behind the sprite.
                float4 finalColor = lerp(layered, original, original.a);
                return finalColor;
            }
            ENDHLSL
        }
        }

            FallBack "Universal Render Pipeline/Unlit"
}
