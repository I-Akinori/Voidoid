Shader "Unlit/IllusionEffectSquare"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
        _Freq ("Freq", float) = 0.5
        _Speed ("Speed", float) = 0.5
        _Offset ("Offset", float) = 0.5
        [MaterialToggle] _Gradation ("Gradation", Float) = 0 
        [MaterialToggle] _InvRotation ("InvRotation", Float) = 0 
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorA;
            float4 _ColorB;
            float4 _Origin;
            float _Freq;
            float _Speed;
            float _Offset;
            float _Gradation;
            float _InvRotation;

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
                float2 uv = i.uv - 0.5;
                float t = fmod(_Time.y, 2);
                float pi = 3.141592;
                float angle = (saturate(t * t) + 0.5) * pi * 0.5;
                angle *= _InvRotation > 0 ? -1 : 1;
                float uvx = cos(angle) * uv.x - sin(angle) * uv.y;
                float uvy = sin(angle) * uv.x + cos(angle) * uv.y;
                uv = float2(uvx, uvy);
                float len = abs(uv.x) + abs(uv.y);
                float v01 = fmod(len * _Freq - (_Time.y + _Offset) * _Speed, 1.);
                v01 = v01 < 0 ? v01 + 1 : v01;
                v01 = 1 - exp(-v01 * 20.0);
                v01 = saturate(v01);
                if (_Gradation == 0)
                {
                    v01 = v01 > 0.75 ? 1 : 0; 
                }
                fixed4 col = lerp(_ColorA, _ColorB, v01);
                // return float4(1, 1, 1, 1);
                return col;
            }
            ENDCG
        }
    }
}
