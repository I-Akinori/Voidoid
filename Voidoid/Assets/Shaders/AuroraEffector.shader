Shader "Unlit/AuroraEffector"
{
    Properties
    {
        _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float _ValueA;
            float _HeightA;
            float4 _ColorB;
            float _ValueB;
            float _HeightB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0, 0, 0, 0);
                float2 p = i.uv * 2.0 - 1.0;
                float rad = p.y;
                float theta = p.x;
                float val = rad;
                float time = -_Time.y * 0.1;
                float border = (sin(theta * 5 + time * 0.1) - 2) * 0.5f;
                border += (sin(theta * 3 - time * 0.23) + 3) * 0.2f;
                border += (sin(theta * 7 + time * 0.87) - 1) * 0.3f;
                border += (sin(theta * 11 + time * 1.31) + 0) * 0.5f;

                float borderA = border * 0.1 * _ValueA;
                borderA += _HeightA;
                float bA = - abs(borderA - val) * 2 + 0.35;
                col += fixed4(_ColorA.xyz, 1) * _ColorA.a * saturate(bA * _ValueA);
                
                float borderB = border * 0.1 * _ValueB;
                borderB += _HeightB;
                float bB = - abs(borderB - val) * 2 + 0.35;
                col += fixed4(_ColorB.xyz, 1) * _ColorB.a * saturate(bB * _ValueB);
                return col;
            }
            ENDCG
        }
    }
}
