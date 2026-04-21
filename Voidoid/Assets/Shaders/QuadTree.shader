Shader "Unlit/QuadTree"
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

            fixed4 frag(v2f i) : SV_Target
            {
                float4 clear = float4(0, 0, 0, 0);
                float4 white = float4(1, 1, 1, 1);
                float4 col = float4(0, 0, 0, 1);
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution.xy);
                p = rotation(p, _Time.y * 0.1);
                col = clear;
                
                float cellSize = 1.0;

                for (uint k = 0; k < 4; k++)
                {
                    int2 q = quotient(p, cellSize);
                    float2 m = mod(p, cellSize);
                    float halfW = 0.005;

                    float rand = GetRandomNumber(q, k + floor(_Time.y));

                    // White
                    if (rand < 0.1 * (k + 1))
                    {
                        col = white;
                        
                        float rand2 = GetRandomNumber(q * 23.632, k + floor(_Time.y));
                        for (uint t = 0; t < 8; t++)
                        {
                            if (rand2 < (t + 1) * 1.0 / 8)
                            {
                                float input = max(saturate(_InputTimes[t] * 0.5), saturate(_InputTimes[t + 8] * 0.5));
                                float4 exCol = float4(HSVtoRGB(float3(t * 1.0 / 8, 1.0, 1.0)), 1);

                                // Heart
                                if (floor(rand2 * 100) % 7 == 0)
                                {
                                    float heart = tex2D(_MainTex, uv_rot(m / cellSize, floor(rand2 / 0.01)));
                                    col = heart * (exCol * input + clear * (1 - input)) + white * (1 - heart);
                                } else
                                {
                                    col = exCol * input + white * (1 - input);
                                }
                                break;
                            }
                        } 
                        
                        if (m.x < halfW || m.x > cellSize - halfW || m.y < halfW || m.y > cellSize - halfW)
                        {
                            col = fixed4(0, 0, 0, 0);
                        }
                        break;
                    }
                    // Brack
                    if (rand > 1 - 0.1 * (k + 1))
                    {
                        break;
                    }
                    // Continue
                    cellSize /= 2;
                }
                return col;
                
                
            }
            ENDCG
        }
    }
}