Shader "Unlit/SimpleEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Resolution("Resolution", Vector) = (1920, 1080, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Resolution; // (width, height, _, _)
            float _EnableVignette;
            float _Bright;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
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
            float Vignette(float2 uv, float2 resolution)
            {
                float2 coord = uv * 2.0 - 1.0;
                float aspect = resolution.x / resolution.y;
                float rangeX = 1;
                float rangeY = 1;

                if (aspect >= 1.0)
                {
                    coord.x *= aspect;
                    rangeX *= aspect;
                }
                else
                {
                    coord.y /= aspect;
                    rangeY /= aspect;
                }
                
                float vigW = 0.1;
                float vignette = 0;
                vignette += rangeX - abs(coord.x);
                vignette += rangeY - abs(coord.y);
                //vignette *= (rangeY - abs(coord.y) + vigW);
                return vignette * 0.7;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                if (_EnableVignette > 0.1f)
                {
                    col *= Vignette(i.uv, _Resolution.xy);
                }
                return col * _Bright;
            }
            ENDCG
        }
    }
}
