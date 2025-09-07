Shader "Ryan/SpriteGrassMask"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Grass look
        _GrassColor ("Grass Color", Color) = (0.25, 0.8, 0.35, 1)
        _Taper ("Blade Taper 0..1", Range(0.0, 1.0)) = 1.0

        // Pixel-sized blades
        _BladeHeightPx ("Blade Height (pixels)", Range(1.0, 256.0)) = 10.0
        _BladeWidthPx  ("Blade Width (pixels)",  Range(0.1, 32.0))  = 0.8

        // Placement
        _CellsX ("Cells X (density)", Range(32, 4096)) = 1024
        _CellsY ("Cells Y (rows)",   Range(1, 4096))   = 1024
        _Coverage ("Blade chance per cell", Range(0.0, 1.0)) = 0.55
        _Seed ("Seed", Float) = 1337.0

        // Mask control
        _MaskBlackThresh ("Mask Black Threshold", Range(0.0, 0.2)) = 0.06
        _BaseBlend ("Show Base Sprite 0..1", Range(0.0, 1.0)) = 0.0
        _RowJitter ("Row Jitter (0..1 of cell)", Range(0.0, 1.0)) = 0.5

        // Wind
        _WindStrength ("Wind Strength", Range(0.0, 0.2)) = 0.05
        _WindFreq     ("Wind Frequency", Range(0.1, 10.0)) = 2.0
        _WindDetail   ("Wind Detail", Range(0.0, 5.0)) = 1.2

        // Orientation
        _NormalGrowBlend   ("Grow Toward Camera 0..1", Range(0.0, 1.0)) = 0.35
        _BillboardGrow     ("Billboard Growth 0..1", Range(0.0, 1.0)) = 1.0
        _NormalProjectDist ("Project Dist (wu)", Range(0.05, 2.0)) = 0.6
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;

                float4 _GrassColor;
                float  _Taper;

                float  _BladeHeightPx;
                float  _BladeWidthPx;

                float  _CellsX;
                float  _CellsY;
                float  _Coverage;
                float  _Seed;

                float  _MaskBlackThresh;
                float  _BaseBlend;
                float  _RowJitter;

                float  _WindStrength;
                float  _WindFreq;
                float  _WindDetail;

                float  _NormalGrowBlend;
                float  _BillboardGrow;
                float  _NormalProjectDist;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 color : COLOR;

                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            // Hash helpers
            float hash11(float n)
            {
                n = frac(n * 0.1031);
                n *= n + 33.33;
                n *= n + n;
                return frac(n);
            }

            float2 hash21(float2 p)
            {
                float3 p3 = frac(float3(p.x, p.y, p.x) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            float luminance(float3 c) { return dot(c, float3(0.2126, 0.7152, 0.0722)); }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldNormal = normalize(TransformObjectToWorldNormal(float3(0,0,1)));
                return o;
            }

            // Use view direction so grass grows toward the camera in screen space
            float2 ComputeUVDirFromWorldNormal(v2f i)
            {
                float3 wP = i.worldPos;
                float3 wDir = normalize(_WorldSpaceCameraPos.xyz - wP);

                float4 clipP  = TransformWorldToHClip(wP);
                float4 clipPn = TransformWorldToHClip(wP + wDir * _NormalProjectDist);

                float2 ndcP  = clipP.xy  / max(1e-6, clipP.w);
                float2 ndcPn = clipPn.xy / max(1e-6, clipPn.w);

                float2 screenP  = (ndcP  * 0.5 + 0.5) * _ScreenParams.xy;
                float2 screenPn = (ndcPn * 0.5 + 0.5) * _ScreenParams.xy;

                float2 gScreen = screenPn - screenP;
                if (dot(gScreen, gScreen) < 1e-6) return float2(0,1);

                float2 dUVdx = ddx(i.uv);
                float2 dUVdy = ddy(i.uv);

                float2 uvDir = dUVdx * gScreen.x + dUVdy * gScreen.y;
                float len = length(uvDir);
                return len > 1e-6 ? uvDir / len : float2(0,1);
            }

            // Map a screen-space direction (in pixels) to a UV direction using local derivatives
            float2 ComputeUVDirFromScreen(float2 screenDir, float2 dUVdx, float2 dUVdy)
            {
                float2 uvDir = dUVdx * screenDir.x + dUVdy * screenDir.y;
                float len = length(uvDir);
                return (len > 1e-6) ? (uvDir / len) : float2(0,1);
            }

            // Convert N screen pixels to UV units along a given UV direction
            float PixelsToUV(float pixels, float2 dir, float2 dUVdx, float2 dUVdy)
            {
                float a = dot(dUVdx, dir);
                float b = dot(dUVdy, dir);
                float perPixel = sqrt(a*a + b*b);
                return pixels * perPixel;
            }

            float bladeAt(
                float2 uv,
                float2 uvHeightDir,
                float2 uvPerpDir,
                int2 cell,
                float2 baseUV,
                float time,
                float heightUVBase,
                float halfWidthUVBase,
                float2 gradX,
                float2 gradY,
                out float shade)
            {
                float4 baseSamp = SAMPLE_TEXTURE2D_GRAD(_MainTex, sampler_MainTex, baseUV, gradX, gradY);
                float3 baseCol = baseSamp.rgb;
                float baseLum = luminance(baseCol);

                // Height factor from luminance: black=1, gray=0.5, white=0
                float maskHeight01 = saturate((1.0 - baseLum) / max(1e-5, (1.0 - _MaskBlackThresh)));
                if (maskHeight01 <= 1e-4)
                {
                    shade = 0.0;
                    return 0.0;
                }

                float2 rnd = hash21(float2(cell) + _Seed);
                float var = lerp(0.85, 1.15, rnd.x);
                float height = heightUVBase * maskHeight01 * var;
                float halfWidth = halfWidthUVBase * lerp(0.9, 1.1, rnd.y);

                float2 d = uv - baseUV;
                float s = dot(d, uvHeightDir);
                if (s < 0.0 || s > height)
                {
                    shade = 0.0;
                    return 0.0;
                }

                float t = time + rnd.x * 6.28318;
                float sway = sin(t * _WindFreq) + 0.5 * sin(t * (_WindFreq * 2.13 + 0.7 * _WindDetail));
                float tilt = sway * _WindStrength * (s / max(height, 1e-4));

                float lateral = dot(d, uvPerpDir) - tilt;

                float taper = lerp(1.0, 1.0 - _Taper, saturate(s / max(height, 1e-4)));
                float halfW = halfWidth * taper;

                float edge = smoothstep(halfW, halfW * 0.7, abs(lateral));
                if (edge <= 0.0)
                {
                    shade = 0.0;
                    return 0.0;
                }

                shade = lerp(0.85, 1.0, saturate(s / max(height, 1e-4)));
                return edge;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 baseSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 baseCol = baseSample * i.color;
                float time = _Time.y;

                float2 dUVdx = ddx(i.uv);
                float2 dUVdy = ddy(i.uv);

                float2 dirViewUV = ComputeUVDirFromWorldNormal(i);
                float2 dirVerticalUV = float2(0, 1);
                float2 dirScreenUpUV = ComputeUVDirFromScreen(float2(0,1), dUVdx, dUVdy);
                float2 dirScreenRightUV = ComputeUVDirFromScreen(float2(1,0), dUVdx, dUVdy);

                float2 uvHeightBase = normalize(lerp(dirVerticalUV, dirViewUV, saturate(_NormalGrowBlend)));
                float2 uvPerpBase = float2(-uvHeightBase.y, uvHeightBase.x);

                float2 uvHeightDir = normalize(lerp(uvHeightBase, dirScreenUpUV, saturate(_BillboardGrow)));
                float2 rightOrtho = normalize(dirScreenRightUV - uvHeightDir * dot(dirScreenRightUV, uvHeightDir));
                float2 uvPerpDir = normalize(lerp(uvPerpBase, rightOrtho, saturate(_BillboardGrow)));

                float heightUV    = PixelsToUV(_BladeHeightPx, uvHeightDir, dUVdx, dUVdy);
                float halfWidthUV = PixelsToUV(_BladeWidthPx * 0.5, uvPerpDir, dUVdx, dUVdy);

                float2 stScale  = _MainTex_ST.xy;
                float2 stOffset = _MainTex_ST.zw;
                float2 uvLocal = (i.uv - stOffset) / max(1e-6, stScale);

                float2 grid = float2(_CellsX, _CellsY);
                float2 cellSizeLocal = 1.0 / grid;
                int2 cell = int2(floor(uvLocal * grid));

                float alpha = 0.0;
                float shade = 0.0;

                [unroll]
                for (int dy = -1; dy <= 1; ++dy)
                {
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        int2 c = int2(cell.x + dx, cell.y + dy);
                        if (c.x < 0 || c.x >= (int)_CellsX || c.y < 0 || c.y >= (int)_CellsY) continue;

                        float chance = hash11(dot(float2(c), float2(17.23, 91.07)) + _Seed * 0.137);
                        if (chance > _Coverage) continue;

                        float2 rnd = hash21(float2(c) + _Seed);

                        float jitterY = (rnd.y - 0.5) * _RowJitter;
                        float2 baseLocal = (float2(c) + float2(rnd.x, jitterY)) * cellSizeLocal;
                        float2 base2 = baseLocal * stScale + stOffset;

                        float localShade;
                        float cov = bladeAt(i.uv, uvHeightDir, uvPerpDir, c, base2, time, heightUV, halfWidthUV, dUVdx, dUVdy, localShade);

                        if (cov > alpha)
                        {
                            alpha = cov;
                            shade = localShade;
                        }
                    }
                }

                // Compose: optionally draw original sprite behind the grass
                if (_BaseBlend > 0.001)
                {
                    float lumBase = luminance(baseSample.rgb);
                    float baseVisible = step(_MaskBlackThresh, lumBase);
                    baseCol.a *= baseVisible * _BaseBlend;
                }
                else
                {
                    baseCol = float4(0,0,0,0);
                }

                // Modulate grass color by sprite tint only
                if (alpha > 0.0)
                {
                    float3 grassRGB = _GrassColor.rgb * i.color.rgb; // no surface modulation
                    grassRGB *= shade;
                    float outA = saturate(baseCol.a + alpha * (1.0 - baseCol.a));
                    float3 outRGB = lerp(baseCol.rgb, grassRGB, alpha);
                    return float4(outRGB, outA);
                }

                return baseCol;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
