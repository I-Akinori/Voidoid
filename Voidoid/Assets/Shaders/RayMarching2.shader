Shader "Unlit/RayMarching2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Resolution("Resolution", Vector) = (1920, 1080, 0, 0)
        _CustomTime("Time", Range(0, 100)) = 0
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

            float cube(float3 p, float3 s)
            {
                float3 q = abs(p);
                float3 m = max(s - q, 0);
                return length(max(q - s, 0)) - min(min(m.x, m.y), m.z);
            }
            float sdCylinder(float3 p, float3 c)
            {
                return length(p.xz - c.xy) - c.z;
            }
            float sdCappedCylinder(float3 p, float3 a, float3 b, float r)
            {
              float3  ba = b - a;
              float3  pa = p - a;
              float baba = dot(ba,ba);
              float paba = dot(pa,ba);
              float x = length(pa*baba-ba*paba) - r*baba;
              float y = abs(paba-baba*0.5)-baba*0.5;
              float x2 = x*x;
              float y2 = y*y*baba;
              float d = (max(x,y)<0.0)?-min(x2,y2):(((x>0.0)?x2:0.0)+((y>0.0)?y2:0.0));
              return sign(d)*sqrt(abs(d))/baba;
            }
            float3 mod(float3 x, float y)
            {
                return x - y * floor(x / y);
            }
            float2 pmod(float2 p, float n)
            {
                
                float rad = length(p);
                float np = 3.141592 * 2. / n;
                float phi = 0;
                if (p.x != 0 || p.y != 0)
                {
                    phi = atan2(p.y, p.x);
                }
                float r = phi - 0.5 * np;
                r = mod(r, np) - 0.5 * np;
                return rad * float2(cos(r), sin(r));
            }
            float2 rotation(float2 p, float rad)
            {
                float x = p.x * cos(rad) - p.y * sin(rad);
                float y = p.x * sin(rad) + p.y * cos(rad);
                return float2(x, y);
            }
            float mainDist(float3 p)
            {
                p.z -= exp(_CustomTime * 0.16) * 15;
                p.xy = rotation(p.xy, _CustomTime * 0.5);
                p.xy = pmod(p.xy, 6);
                p.x -= 0.22;
                p = mod(p, 1) - 0.5;
                //p.z += _Time.y;
                //return sdCappedCylinder(p, float3(0.1, 0.1, 0.1), float3(0.5, 0.4, 0.3), 0.1);
                float d1 = cube(p, float3(0.2, 0.2, 0.2));
                float d2 = cube(p, float3(10.0, 0.05, 0.05));
                float d3 = cube(p, float3(0.05, 10.0, 0.05));
                float d4 = cube(p, float3(0.05, 0.05, 10.0));

                return min(min(d1, d2), min(d3, d4));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);
                
                float3 ro = float3(0, 0, 1);
                float3 ta = float3(0, 0, 0);

                float3 cdir = normalize(ta - ro);
                float3 side = normalize(cross(cdir, float3(0., 1., 0.)));
                float3 up = cross(side, cdir);
                float fov = 3.0;
                float3 rd = normalize(p.x * side + p.y * up + cdir * fov);
                
                float d = 0;
                float t = 0;
                for (uint k = 0; k < 64; k++)
                {
                    d = mainDist(ro + rd * t);
                    if (d < 0.001)
                    {
                        break;
                    }
                    t += d;
                }
                int tar = floor((ro + rd * t).z - exp(_CustomTime * 0.16) * 15) % 8;
                tar = (tar + 8) % 8;
                float3 exCol = HSVtoRGB(float3(tar * 1.0 / 8, 1.0, 1.0));
                float input = saturate(max(_InputTimes[tar], _InputTimes[tar + 8]));
                //exCol = input * exCol + (1 - input) * float3(0, 0, 0);
                col.xyz = exCol;
                col = exp(-0.4 * t);
                return col;
            }
            ENDCG
        }
    }
}