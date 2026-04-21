Shader "Unlit/ImageEffector"
{
    // TextureEffector (壁エフェクトの引継ぎ) → ImageEffector (インタラクティブ)
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DistortionIntensity[16];
            float _NoiseIntensity;
            float _VignetteIntensity;
            float _AberrationIntensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // Distortion
                float2 p = IN.uv * 2 - 1;
                float bright = 1;
                float phi0 = _Time.y * 0.1;
                for (uint i = 0; i < 16; i++)
                {
                    float phi = phi0 + i * 3.141592 / 8;
                    float X = cos(phi + (1 + i % 4));
                    float Y = sin(phi + (1 + i / 4));
                    float2 c = float2(X * 0.8, Y * 0.4);
                    p -= c;
                    float theta = atan2(p.y, p.x);
                    float rad = length(p);
                    theta += _DistortionIntensity[i] * 5 * exp(-rad * 5) * (i % 2 == 0 ? 1 : -1);
                    p.x = rad * cos(theta);
                    p.y = rad * sin(theta);
                    p += c;
                    bright *= saturate(1 - exp(-rad * 5) * _DistortionIntensity[i] * sin(theta * 4 + 5 * _Time.y * (i % 2 == 0 ? 1 : -1)));
                }
                p = p * 0.5 + 0.5;

                float4 col = tex2D(_MainTex, p);
                    
                // Aberration
                if (_AberrationIntensity > 0)
                {
                    float2 downP = p - p % (0.001 * exp(3 * _AberrationIntensity));  
                    col.r = tex2D(_MainTex, downP + _AberrationIntensity * 0.04 * float2(1, 1)).r;
                    col.g = tex2D(_MainTex, downP).g;
                    col.b = tex2D(_MainTex, downP - _AberrationIntensity * 0.04 * float2(1, 1)).b;
                }
                col *= bright;

                // Vignette
                p = IN.uv * 2 - 1;
                float rad = length(p - float2(0, 0.1) * float2(1, 2)) * 1.2;
                col *= lerp(1.0, saturate(0.5 * (1 - rad * _VignetteIntensity)), _VignetteIntensity);
                col.r *= lerp(1.0, 0.0, _VignetteIntensity);
                float wave = 1 + 0.1 * sin(10 * p.x + _Time.y * 0.37) - 0.07 * sin(4.3 * p.x + 0.5 * _Time.y);
                col.g *= lerp(1.0, (p.y + wave) * 0.5, _VignetteIntensity);
                
                return col;
            }
            ENDCG
        }
    }
}
