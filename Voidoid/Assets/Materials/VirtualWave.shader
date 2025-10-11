Shader "Unlit/VirtualWave"
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
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                //fixed4 col = tex2D(_MainTex, i.uv);
                float len = length(i.worldPos.xyz - _Origin.xyz);
                //float v01 = (sin(len * _Freq - _Time.y * _Speed) + 1) * 0.5;
                float v01 = fmod(len * _Freq - _Time.y * _Speed, 1.);
                v01 = v01 < 0 ? v01 + 1 : v01;
                v01 = 1 - exp(-v01 * 20.0);
                v01 = saturate(v01);
                fixed4 col = lerp(_ColorA, _ColorB, v01);
                // return float4(1, 1, 1, 1);
                return col;
            }
            ENDCG
        }
    }
}
