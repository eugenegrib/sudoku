Shader "Custom/GradientTextureShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _GradientOpacity ("Gradient Opacity", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha  // Поддержка альфа-прозрачности
        ZWrite Off  // Отключение записи в Z-буфер для прозрачности

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
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color1;
            fixed4 _Color2;
            sampler2D _MainTex;
            float _GradientOpacity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем цвет текстуры
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Вычисляем цвет градиента
                fixed4 gradientColor = lerp(_Color1, _Color2, i.uv.y);
                
                // Применяем прозрачность градиента
                gradientColor.a *= _GradientOpacity;

                // Определяем яркость пикселя
                float brightness = max(texColor.r, max(texColor.g, texColor.b));

                // Если пиксель светлый, применяем градиент
                if (brightness > 0.1) // Можно регулировать порог
                {
                    // Смешиваем цвет текстуры и градиента
                    return texColor * (1 - gradientColor.a) + gradientColor * gradientColor.a; 
                }

                // Возвращаем исходный цвет, если он темный
                return texColor;
            }
            ENDCG
        }
    }
}
