Shader "Unlit/Grid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
        _Freq ("Freq", float) = 0.5
        _Speed ("Speed", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float4 _Origin;
            float _Freq;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (length(i.uv - 0.5) < 0.02)
                {
                    return _ColorA;
                }
                if (i.uv.x * 8 % 1. < 0.05)
                {
                    return _ColorA;
                }
                if (i.uv.y * 8 % 1. < 0.05)
                {
                    return _ColorA;
                }
                return _ColorB;
            }
            ENDCG
        }
    }
}
