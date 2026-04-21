Shader "Unlit/Menger"
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
            float sdBox(float3 p, float3 b)
            {
                float3 q = abs(p) - b;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
            }
            float sdMenger(float3 p, int iterations)
            {
                float d = sdBox(p, float3(0.5, 0.5, 0.5));
                float scale = 1.0;

                for (int i = 0; i < iterations; i++)
                {
                    float3 q = p * scale;
                    q = frac(q + 0.5) - 0.5;   // 各セルの局所座標へ

                    // このスケールで削る十字穴
                    float holeX = sdBox(q, float3(1.0, 1.0 / 6.0, 1.0 / 6.0));
                    float holeY = sdBox(q, float3(1.0 / 6.0, 1.0, 1.0 / 6.0));
                    float holeZ = sdBox(q, float3(1.0 / 6.0, 1.0 / 6.0, 1.0));

                    float hole = min(holeX, min(holeY, holeZ));

                    d = max(d, -hole / scale);

                    scale *= 3.0;
                }
                return d;
            }

            float mainDist(float3 p, float cellSize)
            {
                return sdMenger(p, 4);
                //return length(p - float3(0, 0, 0)) - 0.25;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                float2 p = i.uv * 2 - 1;
                float3 ro = float3(cos(_Time.y) * 5, sin(_Time.y) * 2, sin(_Time.y) * 5);
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
                        break;
                    }
                    t += d;
                }

                //col.xyz = exCol * input + float3(0, 0, 0) * (1 - input);
                col = float4(1, 1, 1, 1);
                col *= exp(-0.4 * t);
                return col;
            }
            ENDCG
        }
    }
}