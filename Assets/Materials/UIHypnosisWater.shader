Shader "Custom/UIHypnosisWater"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0.05, 0.05, 0.1, 0.8)
        _Speed ("Wave Speed", Float) = 2.0
        _Amount ("Distortion Amount", Float) = 0.03
        _Frequency ("Wave Frequency", Float) = 15.0
        
        // Required for UI
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseMultipleUIElements"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            
            CBUFFER_START(UnityPerMaterial)
            fixed4 _Color;
            float _Speed;
            float _Amount;
            float _Frequency;
            CBUFFER_END
            
            // Глобальная переменная времени без паузы, которую мы передаем из MenuManager.cs
            float _GlobalUnscaledTime;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                // Эффект гипноза/воды на основе времени
                float time = _GlobalUnscaledTime * _Speed;
                
                // Искажение координат
                uv.x += sin(uv.y * _Frequency + time) * _Amount;
                uv.y += cos(uv.x * _Frequency + time) * _Amount;

                // Создаем красивый процедурный узор (плазма/волны), так как у нас нет текстуры!
                float wave = sin(uv.x * 10.0 + time) + cos(uv.y * 10.0 + time);
                
                // Смешиваем базовый цвет (Tint) с нашей волной
                half4 color = IN.color;
                
                // Добавляем переливание (осветляем и затемняем фон по форме волны)
                color.rgb += wave * 0.05; 
                
                // Если вдруг добавите текстуру, она тоже будет искажаться
                color *= tex2D(_MainTex, uv);
                
                return color;
            }
            ENDCG
        }
    }
}
