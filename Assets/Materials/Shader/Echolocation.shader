// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Echolocation"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Center("Center", Vector) = (0,0,0)
        _Radius("Radius", Float) = 0
        _PulseWidth("Pulse Width", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float3 _Center;
            float _Radius;
            float _PulseWidth;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(_Center, i.worldPos);

                // Smooth pulse: fades in and out
                float pulse = smoothstep(_Radius - _PulseWidth, _Radius, dist) 
                            * (1.0 - smoothstep(_Radius, _Radius + _PulseWidth, dist));

                // Multiply by color
                return fixed4(pulse * _Color.rgb, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
