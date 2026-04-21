Shader "Unlit/Star"
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
                p *= 2;
                float rad = length(p);
                float theta = atan2(p.y, p.x) + pi;
                float corners = 5;
                int q = quotient(theta, pi / corners);
                theta = mod(theta, 2 * pi / corners);
                
                if (theta > pi / corners)
                {
                    theta = 2 * pi / corners - theta;
                }

                p.x = rad * cos(theta);
                p.y = rad * sin(theta);
                if (p.y < tan(pi / corners / 2) * (2 * cos(pi / corners) - p.x))
                {
                    sin((q + 1) % 10 / 2 + _Time.y);
                    return fixed4(1, 1, 1, 1);
                }

                return col;
            }
            ENDCG
        }
    }
}