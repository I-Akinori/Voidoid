Shader "Unlit/VoidoidBackground"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Resolution("Resolution", Vector) = (1920, 1080, 0, 0)
        _CubeDance("CubeDance", Range(0, 1)) = 1
        _Julia("Julia", Range(0, 1)) = 1
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
            float _InputIntensity[16];
            float _Julia;
            float _CubeDance;
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
            float3 distToCellBoundary(float3 p, float3 d, float cellSize)
            {
                float3 local = frac(p / cellSize) * cellSize;
                d = normalize(d);

                float3 dist;
                dist.x = (d.x > 0.0) ? (cellSize - local.x) : local.x;
                dist.y = (d.y > 0.0) ? (cellSize - local.y) : local.y;
                dist.z = (d.z > 0.0) ? (cellSize - local.z) : local.z;

                float3 t;
                t.x = (abs(d.x) > 1e-6) ? dist.x / abs(d.x) : 1e30;
                t.y = (abs(d.y) > 1e-6) ? dist.y / abs(d.y) : 1e30;
                t.z = (abs(d.z) > 1e-6) ? dist.z / abs(d.z) : 1e30;

                return min(t.x, min(t.y, t.z)) + 1e-3;;
            }

            // 返り値はcol.rgb + dist
            float4 mainDist(float3 p, float3 d, float cellSize, int gen)
            {
                float4 cold = float4(1, 1, 1, -1);
                float3 c = float3(-0.5 * cellSize, -0.5 * cellSize, - _CustomTime * 1.25 + 0.75);
                float s = 1;
                float3 befModPos = p - c;
                for (uint i = 0; i < gen; i++)
                {
                    float3 modPos = mod(befModPos, cellSize);
                    int3 indexes = floor(befModPos / cellSize);
                    
                    float rand = GetRandomNumber(indexes.xy, indexes.x + indexes.z + 10 * indexes.y);

                    if (i == 0)
                    {
                        int index = indexes.z % 16;
                        float input = _InputIntensity[index];
                        float3 hsv = float3(index / 16.0, 1, 0.5);
                        float3 exCol = HSVtoRGB(hsv);
                        /*
                        switch (index % 4)
                        {
                        case 0:
                            input *= indexes.x > 0 && indexes.y > 0 ? 1 : 0;
                            break;
                        case 1:
                            input *= indexes.x > 0 && indexes.y < 0 ? 1 : 0;
                            break;
                        case 2:
                            input *= indexes.x < 0 && indexes.y < 0 ? 1 : 0;
                            break;
                        case 3:
                            input *= indexes.x < 0 && indexes.y > 0 ? 1 : 0;
                            break;
                        default:
                            break;
                        }
                        */
                        cold.xyz = lerp(float3(1, 1, 1) * 0.1, exCol * 1, input);
                    }
                    if (indexes.x == 0 && indexes.y == 0)
                    {
                        cold.w = distToCellBoundary(befModPos, d, cellSize);
                        return cold;
                    }
                    if (i < gen - 1 && rand < 0.2)
                    {
                        // 再分割
                        cellSize *= 0.5;
                        continue;
                    }
                    if (rand < 0.99 - ((float) i / gen) * 0.5)
                    {
                        cold.w = distToCellBoundary(befModPos, d, cellSize);
                        return cold;
                    }
                    //　キューブが存在
                    float3 d = modPos - 0.5 * cellSize;
                    float3 q = float3(abs(d.x), abs(d.y), abs(d.z)) - 0.5 * s * cellSize;
                    float3 posq = float3(max(0, q.x), max(0, q.y), max(0, q.z));
                    cold.w = length(posq) + min(0, max(q.x, max(q.y, q.z)));
                    return cold;
                }
                return cold;
            }
            float4 cubeDance(float2 uv)
            {
                float4 col = float4(0, 0, 0, 1);
                float2 p = GetShortSideNormalizedCoord(uv, _Resolution.xy);
                float3 ro = float3(cos(_CustomTime) * 0.0, sin(_CustomTime) * 0.0, -1);
                float3 ta = float3(0, 0, 0);

                float3 cdir = normalize(ta - ro);
                float rad = _CustomTime * 0.05;
                float3 side = cross(cdir, float3(cos(rad), sin(rad), 0.));
                float3 up = cross(side, cdir);
                float fov = 0.5;
                float3 rd = normalize(p.x * side + p.y * up + cdir * fov);

                float cellSize = 0.5;
                float4 d = float4(0, 0, 0, 0);
                float t = 0;
                for (uint k = 0; k < 64; k++)
                {
                    d = mainDist(ro + rd * t, rd, cellSize, 4);
                    if (d.w < 0.001)
                    {
                        break;
                    }
                    t += d.w;
                }

                col.xyz = d.xyz;
                col *= exp(-0.4 * t);
                return col;
            }
            float4 julia(float2 uv)
            {
                float2 p = (uv - 0.5) * 5;
                float angle = - _CustomTime * 0.01;
                float px = cos(angle) * p.x - sin(angle) * p.y;
                float py = sin(angle) * p.x + cos(angle) * p.y;
                p = float2(px, py);

                float time = _CustomTime * 0.05;
                int j = 0;
                float2 x = float2(-0.345, 0.654);
                float2 y = float2(0.1 + sin(time) * 0.08, 0.0);
                float2 z = p;
                for (uint k = 0; k < 360; k++)
                {
                    j++;
                    if (length(z) > 2.0) { break; }
                    z = float2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + x + y;
                }
                //float h = abs(fmod(_CustomTime * 15.0 - float(j), 360.0) / 360.0);
                float h = float(j) / 360;
                h = saturate(h - 0.01) * 0.5;
                return float4(h, h, h, 1);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                if (_Julia > 0)
                {
                    col += julia(i.uv) * _Julia;
                }
                if (_CubeDance > 0)
                {
                    col += cubeDance(i.uv) * _CubeDance;
                }
                return col;
            }
            ENDCG
        }
    }
}