Shader "Unlit/StarySky"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            #include "Noise/ClassicNoise2D.hlsl"

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

            float rand(float2 co) //引数はシード値と呼ばれる　同じ値を渡せば同じものを返す
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            float4 star(float2 p, float2 pos, float intensity, float grow, float4 col)
            {
                float dist = length(p - pos) * 1000 * grow;
                float v = 1 - (dist - intensity);
                return saturate(col * v);
            }

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
                // sample the texture
                float4 col = float4(0, 0, 0, 0);
                // apply fog
                int stars = 500;
                
                float2 uv = i.uv - 0.5;
                float angle = _Time.y * 0.1;
                float uvx = cos(angle) * uv.x - sin(angle) * uv.y;
                float uvy = sin(angle) * uv.x + cos(angle) * uv.y;
                uv = float2(uvx, uvy) + 0.5;
                for (int k = 0; k < stars; k++)
                {
                    float x = rand(float2(k, -k));
                    float y = rand(float2(x, k));
                    float2 pos = float2(x, y);
                    float f = rand(pos);
                    float r = rand(float2(y, x));
                    float g = rand(float2(x, r));
                    float b = rand(float2(y, r));
                    col += star(uv, pos, f, f + 0.2,
                        float4(r, g, b, 1) * 0.5 + float4(1, 1, 1, 1) * 0.5);
                }
                float n1 = ClassicNoise(uv * 1 + _Time.y * 0.02) * 2;
                float n2 = ClassicNoise((uv + 2) * 3) * 1;
                float n = ClassicNoise(float2(n1, n2) * 1);
                float v = 0.5 + 0.5 * sin(_Time.y * 0.2 + uv.x);
                float4 cloud = float4(0, 0.3, 0.7, 1) * v
                + float4(0.2, 0.7, 0.2, 1) * (1 - v);
                float mask = ClassicNoise(uv * 1000)
                + ClassicNoise(uv * 5) + ClassicNoise(uv * 10);
                return col + n * n * 1 * cloud * mask;
            }
            ENDCG
        }
    }
}
