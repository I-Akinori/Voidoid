Shader "Unlit/Tube"
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
                p *= 2.5;
                float rad = length(p);
                float theta = atan2(p.y, p.x) + pi;
                float val = -2 * log(rad * 0.3);
                float border = (sin(theta * 5 + _CustomTime * 0.1) + 2) * 0.5f;
                border += (sin(theta * 3 - _CustomTime * 0.23) + 3) * 0.2f;
                border += (sin(theta * 7 + _CustomTime * 0.87) + 1) * 0.3f;
                border += (sin(theta * 11 + _CustomTime * 1.31) + 1) * 0.5f;

                float hightlight = (sin(theta * 4 + _CustomTime * 0.43) + 2) * 0.3f;
                hightlight += (sin(theta * 7 - _CustomTime * 0.83) + 1) * 0.3f;
                hightlight += (sin(theta * 12 - _CustomTime * 0.31) + 2) * 0.3f;
                float bright = sin((border - val) * 1.5 - _CustomTime * 0.5) + 0.75;
                bright *= pow(rad * 0.2, 2);
                bright *= pow(hightlight, 2);
                bright = saturate(bright);
                return fixed4(bright, bright, bright, 1);

                return col;
            }
            ENDCG
        }
    }
}