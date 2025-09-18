Shader "Ryan/SpriteCellShading"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color   ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (0.05,0.10,0.05,1)
        _OutlineWidthPx ("Outline Width (px)", Range(0.5,8)) = 3
        _EdgeFeather ("Edge Feather (px)", Range(0.0,4.0)) = 1.0

        _GrassTint ("Grass Tint Mod", Color) = (0.9,1.0,0.8,1)
        _NoiseScale ("Noise Scale", Range(1,64)) = 12
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.55
        _Seed ("Seed", Float) = 177.0

        _BaseIntensity ("Base Intensity", Range(0,2)) = 1.0
        _OutlineIntensity ("Outline Intensity", Range(0,2)) = 1.0
        _AlphaClip ("Alpha Clip Threshold", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags {"RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SpriteCellShading"
            Tags {"LightMode"="UniversalForward"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize; // x=1/w, y=1/h

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _OutlineColor;
                float4 _GrassTint;
                float _OutlineWidthPx;
                float _EdgeFeather;
                float _NoiseScale;
                float _NoiseStrength;
                float _Seed;
                float _BaseIntensity;
                float _OutlineIntensity;
                float _AlphaClip;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            // Small hash helpers
            float hash11(float n)
            {
                n = frac(n * 0.1031);
                n *= n + 33.33;
                n *= n + n;
                return frac(n);
            }
            float noise2d(float2 p)
            {
                // Simple value noise via hashing 4 corners
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash11(dot(i, float2(127.1, 311.7)) + _Seed);
                float b = hash11(dot(i + float2(1,0), float2(127.1, 311.7)) + _Seed);
                float c = hash11(dot(i + float2(0,1), float2(127.1, 311.7)) + _Seed);
                float d = hash11(dot(i + float2(1,1), float2(127.1, 311.7)) + _Seed);
                float2 u = f*f*(3.0-2.0*f);
                return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
            }

            float sampleAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            float edgeDistance(float2 uv, float maxRadiusPx)
            {
                // Approximate distance in pixels to nearest opaque pixel using concentric rings
                // Early-out at first hit. Falls back to maxRadius if none.
                float baseA = sampleAlpha(uv);
                if (baseA > _AlphaClip) return 0; // inside already

                float2 texel = _MainTex_TexelSize.xy; // uv units per pixel
                int maxR = (int)ceil(maxRadiusPx);
                for (int r = 1; r <= maxR; r++)
                {
                    int steps = (int)(6.28318 * r); // circumference samples
                    float invSteps = 1.0 / max(1, steps);
                    for (int s = 0; s < steps; s++)
                    {
                        float a = (s + 0.5) * invSteps * 6.28318;
                        float2 offsetPx = float2(cos(a), sin(a)) * r;
                        float2 offsetUV = offsetPx * texel;
                        if (sampleAlpha(uv + offsetUV) > _AlphaClip)
                            return r; // found nearest opaque
                    }
                }
                return maxRadiusPx + 1; // none inside radius
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                float alpha = baseCol.a;

                float outlineWidthPx = _OutlineWidthPx;

                // Determine if this pixel should show outline
                float distPx = edgeDistance(i.uv, outlineWidthPx);
                bool inside = (alpha > _AlphaClip);
                bool drawOutline = (!inside) && distPx <= outlineWidthPx;

                if (!inside && !drawOutline)
                {
                    // Fully transparent otherwise
                    return float4(0,0,0,0);
                }

                if (inside)
                {
                    // Base sprite modulated by intensity
                    baseCol.rgb *= _BaseIntensity;
                }

                if (drawOutline)
                {
                    // Normalize distance so 0 = edge, 1 = outer border
                    float t = saturate(distPx / max(outlineWidthPx, 0.001));

                    // Feather inward using _EdgeFeather
                    float feather = (_EdgeFeather > 0) ? smoothstep(0, _EdgeFeather, outlineWidthPx - distPx) : 1.0;

                    // Noise modulation for grassy irregular edge
                    float n = noise2d(i.uv * _NoiseScale);
                    float irregular = lerp(1.0, n, _NoiseStrength);

                    float3 oCol = _OutlineColor.rgb * _GrassTint.rgb * irregular;

                    // Slight darken toward outer edge (like blade taper) and brighten near inner contact
                    float shade = lerp(0.65, 1.1, 1.0 - t);
                    oCol *= shade;

                    float outA = feather * _OutlineColor.a * _OutlineIntensity;
                    return float4(oCol, outA);
                }

                return float4(baseCol.rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
