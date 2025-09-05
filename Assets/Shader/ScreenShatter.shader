Shader "Ryan/ScreenShatter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Progress ("Progress 0..1", Range(0,1)) = 0
        _Explode  ("Explode Distance", Float) = 1
        _Spin     ("Spin Radians", Float) = 6.28318
        _Jitter   ("Jitter Distance", Float) = 0.5
        _CenterUV ("Center in UV", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Progress;
            float _Explode;
            float _Spin;
            float _Jitter;
            float4 _CenterUV;

            float hash11(float x){ return frac(sin(x * 12.9898) * 43758.5453); }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1; // shard id in x
                float2 uv3    : TEXCOORD2; // shard center uv
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float  a   : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float id = v.uv2.x;
                float r0 = hash11(id);
                float r1 = hash11(id + 17.0);
                float r2 = hash11(id + 41.0);

                float explodeAmt = _Explode * _Progress;

                // Direction outwards from shard center toward effect center
                float2 dirUV = normalize((v.uv3 - _CenterUV.xy) + 1e-4);

                // Rotate around shard center (in clip-like space)
                float angle = _Spin * _Progress * (r0 * 2.0 - 1.0);
                float s = sin(angle);
                float c = cos(angle);

                // Current vertex in clip-like space
                float2 p = v.vertex.xy;

                // Compute pivot in same space from v.uv3
                float2 pivot = float2(v.uv3.x * 2.0 - 1.0, v.uv3.y * 2.0 - 1.0);

                // Pivot -> rotate -> unpivot
                p -= pivot;
                float2 pr = float2(c * p.x - s * p.y, s * p.x + c * p.y);
                p = pr + pivot;

                // Add explode and jitter
                float2 explodeXY = dirUV * explodeAmt;
                float2 jitterXY = float2(r1 - 0.5, r2 - 0.5) * _Jitter * _Progress;

                o.pos = float4(p + explodeXY + jitterXY, 0, 1);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.a = 1.0; // keep opaque for now
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= i.a;
                return col;
            }
            ENDCG
        }
    }
}
