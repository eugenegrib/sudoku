Shader "Custom/BlurShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
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
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Нормализация размера размытия в зависимости от размеров текстуры
                float2 blurOffset = _BlurSize / _ScreenParams.xy;

                // Простое размытие
                col += tex2D(_MainTex, i.uv + blurOffset * float2(1, 0));
                col += tex2D(_MainTex, i.uv - blurOffset * float2(1, 0));
                col += tex2D(_MainTex, i.uv + blurOffset * float2(0, 1));
                col += tex2D(_MainTex, i.uv - blurOffset * float2(0, 1));
                col /= 5.0; // Нормализация
                return col;
            }
            ENDCG
        }
    }
}
