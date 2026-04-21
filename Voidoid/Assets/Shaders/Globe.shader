Shader "Unlit/Globe"
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

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            struct hit_info
            {
                float dist;
                int target;
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
            float sdTorus(float3 p, float2 t)
            {
              float2 q = float2(length(p.xz) - t.x, p.y);
              return length(q) - t.y;
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
            hit_info mainDist(float3 p)
            {
                float3 q = p;
                float pi = 3.141592;
                hit_info hit;
                float d = sdTorus(q, float2(1, 0.01));
                int t = -1;
                for (int i = 0; i < 3; i++)
                {
                    float phi = pi / 2 * (i + 1) / 4;
                    float3 r = q;
                    r.y = q.y + sin(phi);
                    d = min(d, sdTorus(r, float2(cos(phi), 0.01)));
                    
                    r.y = q.y - sin(phi);
                    d = min(d, sdTorus(r, float2(cos(phi), 0.01)));
                }

                q = p;
                q.yz = rotation(p.yz, pi * 0.5);
                
                for (int i = 0; i < 8; i++)
                {
                    q.xy = rotation(q.xy, pi / 8);
                    if (d > sdTorus(q, float2(1, 0.01)))
                    {
                        d = sdTorus(q, float2(1, 0.01));
                        t = i;
                    }
                }
                hit.dist = d;
                hit.target = t;
                return hit;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = float4(0, 0, 0, 1);
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);

                p *= 2;
                
                float theta = _CustomTime;
                float rad = 2 + cos(theta * 1.37) * 0.5;
                float3 ro = rad * float3(cos(theta), sin(theta) * 0.3, sin(theta));
                float3 ta = float3(0, 0, 0);

                float3 cdir = normalize(ta - ro);
                float3 side = normalize(cross(cdir, float3(0., 1., 0.)));
                float3 up = cross(side, cdir);
                float fov = 2.0;
                float3 rd = normalize(p.x * side + p.y * up + cdir * fov);
                
                float d = 0;
                float t = 0;
                hit_info hit;
                for (uint k = 0; k < 64; k++)
                {
                    hit = mainDist(ro + rd * t);
                    if (hit.dist < 0.001)
                    {
                        break;
                    }
                    t += hit.dist;
                }
                float3 exCol = float3(1, 1, 1);
                if (hit.target >= 0)
                {
                    int tar = (hit.target + 8) % 8;
                    exCol = HSVtoRGB(float3(tar * 1.0 / 8, 1.0, 1.0));
                    float input = saturate(max(_InputTimes[tar], _InputTimes[tar + 8]));
                    exCol = input * exCol + (1 - input) * float3(1, 1, 1);
                }
                col.xyz = exCol;
                col.a = exp(-0.4 * t);
                return col;
            }
            ENDCG
        }
    }
}