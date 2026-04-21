Shader "Unlit/Landscape"
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
            #include "Utility/ColorConverter.hlsl"
            #include "Utility/PerlinNoise2D.hlsl"

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
                float2 coord = uv * 2.0 - 1.0;
                float aspect = resolution.x / resolution.y;

                if (aspect >= 1.0)
                {
                    coord.x *= aspect;
                }
                else
                {
                    coord.y /= aspect;
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
            float2 mod(float2 x, float y)
            {
                return x - y * floor(x / y);
            }
            int2 quotient(float2 x, float y)
            {
                return floor(x / y);
            }
            float2 rotation(float2 p, float rad)
            {
                float x = p.x * cos(rad) - p.y * sin(rad);
                float y = p.x * sin(rad) + p.y * cos(rad);
                return float2(x, y);
            }
            float2 uv_rot(float2 uv, int times)
            {
                for (uint i = 0; i < times; i++)
                {
                    uv = float2(1 - uv.y, uv.x);
                }
                return uv;
            }
            float4 lerp(float4 s, float4 g, float v)
            {
                v = saturate(v);
                return s * (1 - v) + g * v;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 clear = float4(0, 0, 0, 0);
                float4 white = float4(1, 1, 1, 1);
                float4 col = clear;
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);
                //p = rotation(p, _Time.y * 0.1);
                
                // Sky
                float input = max(max(_InputTimes[4], _InputTimes[9]), _InputTimes[14]);
                float4 exCol = float4(1.00, 0.70, 0.40, 1);
                exCol = lerp(white * 0.5, exCol, input);
                col = exCol * (1 - i.uv.y);
                float2 i2 = i.uv;
                i2.x += _CustomTime * 0.01;
                // cloud
                col += white * 0.2 * saturate(
                    perlinNoise2D(i2 * float2(20, 25)) * 0.5 +
                    perlinNoise2D(i2 * float2(10, 15)) * 1.0 +
                    perlinNoise2D(i2 * float2(3, 4)) * 2.5 - 2.0
                    )
                * saturate(1 - 20 * (i2.y - 0.5) * (i2.y - 0.5)) * saturate(_CustomTime - 4);
                // Mountatin
                if (p.y < sin(p.x * 3 + 4) * 0.15 + sin(p.x * 1 + 5) * 0.2 + 0)
                {
                    input = max(max(_InputTimes[3], _InputTimes[8]), _InputTimes[13]);
                    exCol = float4(0.10, 0.20, 0.00, 1);
                    exCol = lerp(white * 0.2, exCol, input);
                    col = lerp(col, exCol, (_CustomTime - 3.5) * 0.5);
                }
                // Opposite land
                if (p.y < sin(p.x * 2 + 4) * 0.05 -0.4)
                {
                    input = max(max(_InputTimes[2], _InputTimes[7]), _InputTimes[12]);
                    exCol = float4(0.30, 0.40, 0.10, 1);
                    exCol = lerp(white * 0.7, exCol, input);
                    col = lerp(col, exCol, (_CustomTime - 3) * 0.5);
                }
                // River
                if (p.y < sin(p.x * 2 + _CustomTime) * 0.03 - 0.6)
                {
                    input = max(max(_InputTimes[1], _InputTimes[6]), _InputTimes[11]);
                    exCol = float4(0.25, 0.20, 0.35, 1);
                    exCol = lerp(white * 0.5, exCol, input);
                    col = lerp(col, exCol, (_CustomTime - 2.5) * 0.5);
                }
                // Front land
                if (p.y < sin(p.x * 2 + 1) * 0.02f - 0.8)
                {
                    input = max(max(_InputTimes[0], _InputTimes[5]), max(_InputTimes[10], _InputTimes[15]));
                    exCol = float4(0.25, 0.30, 0.00, 1);
                    exCol = lerp(white * 0.7, exCol, input);
                    col = lerp(col, exCol, (_CustomTime - 2) * 0.5);
                }
                return col;
            }
            ENDCG
        }
    }
}