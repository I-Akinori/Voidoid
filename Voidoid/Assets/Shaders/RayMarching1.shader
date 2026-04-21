Shader "Unlit/RayMarching1"
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

            sampler2D _MainTex;
            float4 _Resolution; // (width, height, _, _)
            float _CustomTime;
            float _InputTimes[16];
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float GetRandomNumber(float2 texCoord, int Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // transform to [0,1]
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

            float3 mod(float3 x, float y)
            {
                return x - y * floor(x / y);
            }

            int3 quotient(float3 x, float y)
            {
                return floor(x / y);
            }

            float mainDist(float3 p, float cellSize)
            {
                p.z += _Time.y;
                return length(mod(p, cellSize) - 0.5 * cellSize) - 0.25 * cellSize;
                //return length(p - float3(0, 0, 0)) - 0.25;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);
                float3 ro = float3(cos(_Time.y) * 0.2, sin(_Time.y) * 0.2, -1);
                float3 ta = float3(0, 0, 0);

                float3 cdir = normalize(ta - ro);
                float rad = _Time.y * 0.1;
                float3 side = cross(cdir, float3(cos(rad), sin(rad), 0.));
                float3 up = cross(side, cdir);
                float fov = 2.0;
                float3 rd = normalize(p.x * side + p.y * up + cdir * fov);

                float cellSize = 1.0;
                float d = 0;
                float t = 0;
                for (uint k = 0; k < 64; k++)
                {
                    d = mainDist(ro + rd * t, cellSize);
                    if (d < 0.001)
                    {
                        return fixed4(1, 0, 0, 1);
                        break;
                    }
                    t += d;
                }

                int3 q = quotient(ro + rd * t, cellSize);
                int tar = floor(GetRandomNumber(q.xy * 0.1, abs(q.z)) * 8);
                float input = max(saturate(_InputTimes[tar] * 0.5), saturate(_InputTimes[tar + 8] * 0.5));
                float3 exCol = HSVtoRGB(float3(tar * 1.0 / 8, 1.0, 1.0));

                col.xyz = exCol * input + float3(0, 0, 0) * (1 - input);
                col.a = exp(-0.4 * t);
                return col;
            }
            ENDCG
        }
    }
}