Shader "Custom/EcholocationOutline"
{
    Properties
    {
        _Color ("Echo Color", Color) = (1,1,1,1)
        _PulseWidth ("Pulse Width", Float) = 1.5
        _EdgePower ("Edge Strength", Float) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _PulseWidth;
            float _EdgePower;

            float3 _Center;
            float _Radius;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _Center);

                // Sound ring
                float ring =
                    smoothstep(_Radius - _PulseWidth, _Radius, dist) *
                    (1 - smoothstep(_Radius, _Radius + _PulseWidth, dist));

                // Edge detection
                float edge = pow(1 - abs(dot(normalize(i.normal), normalize(i.worldPos - _Center))), _EdgePower);

                float alpha = ring * edge;

                clip(alpha - 0.01);

                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
