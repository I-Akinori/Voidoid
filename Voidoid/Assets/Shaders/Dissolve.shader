Shader "Unlit/Dissolve"
{
    Properties
    {
        _Threshold ("Threshold", Range(0, 2)) = 0.5
        _Origin ("Origin", Vector) = (0, 0, 0, 0)
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
                float4 worldPosSrc : TEXCOORD1;
                float4 worldPosDis : TEXCOORD2;
            };

            float _Threshold;
            float4 _Origin;
            float4 _Axis;
            
            float4 _ColorA;
            float4 _CenterA;
            float _FreqA;
            float _SpeedA;
            float _ValueA;
            
            float4 _ColorB;
            float4 _CenterB;
            float _FreqB;
            float _SpeedB;
            float _ValueB;
            
            float4 _ColorC;
            float4 _CenterC;
            float _FreqC;
            float _SpeedC;
            float _ValueC;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPosSrc = worldPos;
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
                t += r * _Threshold * _Threshold * _Threshold * 0.2;
                float x2 = r * cos(t);
                float y2 = r * sin(t);
                float3 p = axisX * x2 + axisY * y2 + axisZ * z2;
                o.worldPosDis = float4(p, 0) + _Origin;
                //o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                
                return o;
            }
            float4 sphere_wave(float4 pos, float4 cen, float freq, float speed, float val, float4 col)
            {
                float len = length(pos.xyz - cen.xyz) / (0.001 + val);
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
                float4 col = sphere_wave(i.worldPosSrc, _CenterA, _FreqA, _SpeedA, _ValueA, _ColorA);
                col *= sphere_wave(i.worldPosSrc, _CenterB, _FreqB, _SpeedB, _ValueB, _ColorB);
                col *= sphere_wave(i.worldPosSrc, _CenterC, _FreqC, _SpeedC, _ValueC, _ColorC);
                
                float v = 1 + SimplexNoise(SimplexNoiseGrad(i.worldPosDis * 0.5) * 0.25);
                v *= 0.25;
                float width = _Threshold * 0.75;

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
