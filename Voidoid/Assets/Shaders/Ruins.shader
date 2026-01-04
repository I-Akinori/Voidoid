Shader "Unlit/Ruins"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
        _Freq ("Freq", float) = 0.5
        _Speed ("Speed", float) = 0.5
        _Threshold ("Threshold", Range(0, 1)) = 0.5
        _BlockX ("BlockX", Range(1, 128)) = 8
        _BlockY ("BlockY", Range(1, 128)) = 8
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
            #include "Noise/ClassicNoise2D.hlsl"

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
            float _Threshold;
            int _BlockX;
            int _BlockY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            float generateValue(float2 co)
            {
                float v = ClassicNoise(co * 8);
                v += ClassicNoise(co * 16) * 0.5;
                v = (v + 1.5) / 3.0;
                v += 0.25;
                v *= co.y;
                v *= rand(co) * 0.5 + 0.5;
                return v;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float4 colA = _ColorA;
                float4 colB = col;//_ColorB;
                float k = generateValue(i.uv);
                if (k > _Threshold)
                {
                    return col;
                }
                int blockX = _BlockX;
                int blockY = _BlockY;
                if (i.uv.x * blockX % 1. < 0.05
                    || i.uv.x * blockX % 1. > 1.0 - 0.05
                    || i.uv.y * blockY % 1. < 0.05
                    || i.uv.y * blockY % 1. > 1.0 - 0.05)
                {
                    //return _ColorA;
                    return fixed4(colA.xyz, k + 0.1 < _Threshold ? 0 : 1);
                }
                int x = floor(i.uv.x * blockX / 1.);
                int y = floor(i.uv.y * blockY / 1.);
                
                float v = generateValue(float2((float)x / blockX, (float)y / blockY));
                float w = 0.1;
                float thr = _Threshold - w;
                
                if (v >= thr)
                {
                    float a = v - thr;
                    if (a <= w)
                    {
                        a = a / w;
                    } else
                    {
                        a = 1;
                    }
                    return lerp(colA, colB, a);
                }
                return colA;
            }
            ENDCG
        }
    }
}
