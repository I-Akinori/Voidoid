Shader "Unlit/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold ("Threshold", Range(0, 2)) = 0.5
        _Origin ("Origin", Vector) = (0, 0, 0, 1)
        _Axis ("Axis", Vector) = (0, 0, 1, 0)
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Noise/SimplexNoise3D.hlsl"

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
            float _Threshold;
            float4 _Origin;
            float4 _Axis;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos -= _Origin;
                float3 axisZ = normalize(_Axis.xyz);
                float3 axisX = normalize(cross(axisZ, float3(1, 0, 0)));
                float3 axisY = cross(axisZ, axisX);
                float x = dot(worldPos.xyz, axisX);
                float y = dot(worldPos.xyz, axisY);
                float z2 = dot(worldPos.xyz, axisZ);
                float r = sqrt(x * x + y * y);
                float t = atan2(y, x);
                //t += 3.141592 * 0.25;
                t += r * _Threshold * _Threshold * 2;
                float x2 = r * cos(t);
                float y2 = r * sin(t);
                float3 p = axisX * x2 + axisY * y2 + axisZ * z2;
                o.worldPos = float4(p, 0) + _Origin;
                //o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float v = 1 + SimplexNoise(SimplexNoiseGrad(i.worldPos * 0.5) * 0.25);
                v *= 0.5;
                if (length(i.worldPos.xy) < 1)
                {
                    //return fixed4(0, 0, 1, 1);
                }
                if (i.worldPos.y < 0)
                {
                    //return fixed4(1, 0, 0, 1);
                }
                if (i.worldPos.z > 0)
                {
                    //return fixed4(0, 1, 0, 1);
                }
                float width = _Threshold * 0.4;

                if (v < _Threshold)
                {
                    v -= _Threshold - width;
                    if (v > 0)
                    {
                        col.a = v / width;
                    } else
                    {
                        col.a = 0;
                    }
                } else
                {
                    col.a = 1;
                }
                return col;
            }
            ENDCG
        }
    }
}
