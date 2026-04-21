Shader "Unlit/OutlineEffector"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
        _Axis ("Axis", Vector) = (0, 0, 1, 0)
        _Height ("Height", Range(0, 1)) = 0.5
        _Value ("Value", Range(0, 1)) = 0.5
        _Dissolve ("Dissolve", Range(0, 1)) = 0.5
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

            float4 _Color;
            float4 _Origin;
            float4 _Axis;
            float _Height;
            float _Value;
            float _Dissolve;

            v2f vert (appdata v)
            {
                v2f o;
                float h = v.uv.y;
                float w = sqrt(h * (1 - h));
                float a = w * exp(-0.5 * (h - _Height) * (h - _Height) * 20) * 20;
                float d = a * sin(_Value * 5 * 3.141592) * _Value;
                v.vertex += float4(1, 0, 0, 0) * d;
                
                o.uv = v.uv;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos -= _Origin;
                float3 axisZ = normalize(_Axis.xyz);
                float3 axisX = normalize(cross(axisZ, float3(1, 0, 0)));
                float3 axisY = cross(axisZ, axisX);
                float x = dot(worldPos.xyz, axisX);
                float y = dot(worldPos.xyz, axisY);
                float z = dot(worldPos.xyz, axisZ);
                float r = sqrt(x * x + y * y);
                float t = atan2(y, x);
                t += r * pow(_Dissolve, 3) * 4;
                x = r * cos(t);
                y = r * sin(t);
                z += pow(_Dissolve, 2) * 8;
                float3 p = axisX * x + axisY * y + axisZ * z;
                worldPos = float4(p, 0) + _Origin;
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0, 0, 0, 1);
                float h = i.uv.y;
                float a = exp(-0.5 * (h - _Height) * (h - _Height) * 40) * 5;
                col = lerp(col, _Color, a * _Value);
                col.a *= 1 - _Dissolve;
                return col;
            }
            ENDCG
        }
    }
}
