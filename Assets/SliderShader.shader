Shader "Unlit/SliderShader"
{
    Properties
    {
        _Percent("Percent", Float) = 0.6666
        _Roundness("Roundness", Float) = 1
        _ColorFilled ("Color filled", Color) = (0,1,0,1)
        _ColorEmpty ("Color empty", Color) = (1,0,0,1)
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

            float4 _ColorFilled;
            float4 _ColorEmpty;
            float _Percent;
            float _Roundness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float xcoord = i.uv; // between 0,1
                float distance = length(i.uv-float2(_Percent,0.5));
                distance = step(distance,_Roundness);
                float thisPixel = step(i.uv,_Percent);
                thisPixel = max(distance,thisPixel);

                fixed4 col = lerp(_ColorEmpty,_ColorFilled,thisPixel);
                return distance;
                //return float4(i.uv.xy,0,1);
            }
            ENDCG
        }
    }
}
