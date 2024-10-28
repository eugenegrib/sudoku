Shader "UI/HorizontalGradientWithImage"
{
    Properties
    {
        _LeftColor ("Left Color", Color) = (1, 1, 1, 1)
        _RightColor ("Right Color", Color) = (0, 0, 0, 1)
        _MainTex ("Source Image", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1 // Добавляем альфа-канал
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        Lighting Off
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _LeftColor;
            fixed4 _RightColor;
            sampler2D _MainTex; // Текстура кнопки
            fixed4 _Color; // Цвет кнопки (если используется)
            float _Alpha; // Альфа-значение

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Получаем пиксели из Source Image
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Применяем градиент по оси X
                fixed4 gradientColor = lerp(_LeftColor, _RightColor, i.uv.x);

                // Умножаем цвет текстуры на градиент
                // И учитываем альфа-канал
                return texColor * gradientColor * _Alpha;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
