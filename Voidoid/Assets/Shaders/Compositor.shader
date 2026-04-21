Shader "Unlit/Compositor"
{
    Properties
    {
        _BaseTex("Texture", 2D) = "white" {}
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
            float _OverRatio;
            uint _CompositeType;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float GetRandomNumber(float2 texCoord, int Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy; // transform to [0,1]
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BaseTex, i.uv);
                fixed4 over = tex2D(_OverTex, i.uv);
                switch (_CompositeType)
                {
                    // Alpha blend
                    case 0:
                        col.xyz = col.xyz * (1 - over.a * _OverRatio)
                                    + over.xyz * over.a * _OverRatio;
                        break;
                    // Additional blend
                    case 1:
                        col.xyz = col.xyz + over.xyz * over.a * _OverRatio;
                        break;
                    // Masking
                    case 2:
                        col = col * over.a * _OverRatio;
                        break;
                    default:
                        //col.xyz = (1 - col.xyz);
                        col.xyz = (1 - col.xyz) * over.a * _OverRatio
                                    + col.xyz * (1 - over.a * _OverRatio);
                        break;
                }
                col.a = 1;
                return col;
            }
            ENDCG
        }
    }
}