Shader "Unlit/Steel"
{
    Properties
    {
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

            sampler2D _BaseTex;
            sampler2D _OverTex;
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
            float mod(float x, float y)
            {
                return x - y * floor(x / y);
            }
            int quotient(float x, float y)
            {
                return floor(x / y);
            }
            float2 rotation(float2 p, float rad)
            {
                float x = p.x * cos(rad) - p.y * sin(rad);
                float y = p.x * sin(rad) + p.y * cos(rad);
                return float2(x, y);
            }
            float arrow(float2 p, float height, float width) {
                p.x = abs(p.x);
                if (p.y > 0 && p.y - height < - 2 * height / width *(p.x - width / 2) )
                {
                    return 1;
                }
                return 0;
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(0, 0, 0, 0);
                float pi = 3.141592;
                float2 p = GetShortSideNormalizedCoord(i.uv, _Resolution);
                float keyPoints[] = 
                {
                    2, 2.1, 2.3, 2.4, 3
                };
                float vanishTime = 4;
                for (uint j = 0; j < 5; j++)
                {
                    float rand1 = GetRandomNumber(float2(1, 5), j);
                    float rand2 = GetRandomNumber(float2(2, 7), j);
                    float rand3 = GetRandomNumber(float2(5, 1), j);
                    float2 q = p;
                    q = rotation(q, j * 1 + rand1 * 0.2);
                    q *= 2;
                    q.y += -saturate((_CustomTime - keyPoints[j]) * 5) * 2 + 2.0;
                    
                    col += arrow(q, 0.3 + rand2 * 0.2, 0.05 + rand3 * 0.1)
                    * saturate(_CustomTime - keyPoints[j] + 0.2) * saturate(4 - _CustomTime);
                }

                return col;
            }
            ENDCG
        }
    }
}