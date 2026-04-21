Shader "Unlit/Noise"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Resolution("Resolution", Vector) = (1920, 1080, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Utility/Noise1D.hlsl"

            sampler2D _MainTex;
            float4 _Resolution; // (width, height, _, _)
            float _CustomTime;
            float _InputTimes[16];

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float GetRandomNumber(float2 texCoord, int Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy; // transform to [0,1]
                return o;
            }

            float2 GetShortSideNormalizedCoord(float2 uv, float2 resolution)
            {
                float2 coord = uv * 2.0 - 1.0; // [-1,1] に中心化
                float aspect = resolution.x / resolution.y;

                // ✅ 短辺を基準に長辺を補正（←重要ポイント）
                if (aspect >= 1.0)
                {
                    coord.x *= aspect; // 横長ならxを圧縮
                }
                else
                {
                    coord.y /= aspect; // 縦長ならyを拡張
                }

                return coord;
            }
            float2 ShiftCoord(float2 xy, float t)
            {
                return xy + t;
            }
            float2 ExpandCoord(float2 xy)
            {
                return xy * sqrt(length(xy)) * 0.5;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                float2 coord = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);
                coord = ExpandCoord(coord);
                
                coord.x += 0.010 * GradientNoise(coord.y * 100, _CustomTime * 5.31);
                coord.x += 0.020 * GradientNoise(coord.y * 10, _CustomTime * 1.93);
                coord = ShiftCoord(coord, _CustomTime * 0.1);
                
                float2 check = floor(coord * 10.0);
                float c = fmod(check.x + check.y, 2.0);
                col = c == 0 ? fixed4(1, 1, 1, 1) : fixed4(0, 0, 0, 0);
                //col *= i.uv.x;
                return col;
            }
            ENDCG
        }
    }
}