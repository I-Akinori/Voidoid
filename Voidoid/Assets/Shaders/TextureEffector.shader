Shader "Unlit/TextureEffector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        _CenterA ("CenterA", Vector) = (0, 0, 0, 0)
        _FreqA ("FreqA", float) = 0.5
        _SpeedA ("SpeedA", float) = 0.5
        _ValueA ("ValueA", Range(0, 1)) = 0.5
        
        _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _CenterB ("CenterB", Vector) = (0, 0, 0, 0)
        _FreqB ("FreqB", float) = 0.5
        _SpeedB ("SpeedB", float) = 0.5
        _ValueB ("ValueB", Range(0, 1)) = 0.5
        
        _ColorC ("ColorC", Color) = (1, 1, 1, 1)
        _CenterC ("CenterC", Vector) = (0, 0, 0, 0)
        _FreqC ("FreqC", float) = 0.5
        _SpeedC ("SpeedC", float) = 0.5
        _ValueC ("ValueC", Range(0, 1)) = 0.5
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
            
            float4 _ColorA;
            float2 _CenterA;
            float _FreqA;
            float _SpeedA;
            float _ValueA;
            
            float4 _ColorB;
            float2 _CenterB;
            float _FreqB;
            float _SpeedB;
            float _ValueB;
            
            float4 _ColorC;
            float2 _CenterC;
            float _FreqC;
            float _SpeedC;
            float _ValueC;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float4 circle_wave(float2 pos, float2 cen, float freq, float speed, float val, float4 col)
            {
                float len = length(pos.xy - cen.xy) / (0.001 + val);
                float v01 = 0;
                if (len * freq - val * speed < 3)
                {
                    v01 = fmod(len * freq - val * speed, 1.0);
                    v01 = v01 < 0 ? v01 + 1 : v01;
                    v01 = exp(-v01 * 2.0) * (1 - val);
                    v01 = saturate(v01);
                }
                float4 white = float4(1, 1, 1, 1);
                return lerp(white, col, v01);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 waveCol = circle_wave(i.uv, _CenterA, _FreqA, _SpeedA, _ValueA, _ColorA);
                waveCol *= circle_wave(i.uv, _CenterB, _FreqB, _SpeedB, _ValueB, _ColorB);
                waveCol *= circle_wave(i.uv, _CenterC, _FreqC, _SpeedC, _ValueC, _ColorC);
                col *= waveCol;
                //col.xyz = lerp(col.xyz, waveCol.xyz, waveCol.a);
                return col;
            }
            ENDCG
        }
    }
}
