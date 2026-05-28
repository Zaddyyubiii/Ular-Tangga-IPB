Shader "UI/BackdropBlur"
{
    Properties
    {
        _Size ("Blur Size", Range(0, 20)) = 5.0
        _Color ("Tint Color", Color) = (0, 0, 0, 0.2)
        
        // Required for UI.Image compatibility
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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

        // Grab the screen behind the UI element
        GrabPass
        {
            "_BackgroundTexture"
        }

        Pass
        {
            Name "BackdropBlurPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

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
                float2 texcoord : TEXCOORD0;
                float4 grabPos  : TEXCOORD1;
            };

            sampler2D _BackgroundTexture;
            float4 _BackgroundTexture_TexelSize;
            float _Size;
            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                float depth = _Size * 0.001;

                // 9-tap Gaussian Box Blur sampling the GrabPass texture
                fixed4 sum = fixed4(0,0,0,0);
                
                sum += tex2D(_BackgroundTexture, uv + float2(-1.0, -1.0) * depth) * 0.09;
                sum += tex2D(_BackgroundTexture, uv + float2(0.0, -1.0) * depth) * 0.12;
                sum += tex2D(_BackgroundTexture, uv + float2(1.0, -1.0) * depth) * 0.09;
                
                sum += tex2D(_BackgroundTexture, uv + float2(-1.0, 0.0) * depth) * 0.12;
                sum += tex2D(_BackgroundTexture, uv + float2(0.0, 0.0) * depth) * 0.16;
                sum += tex2D(_BackgroundTexture, uv + float2(1.0, 0.0) * depth) * 0.12;
                
                sum += tex2D(_BackgroundTexture, uv + float2(-1.0, 1.0) * depth) * 0.09;
                sum += tex2D(_BackgroundTexture, uv + float2(0.0, 1.0) * depth) * 0.12;
                sum += tex2D(_BackgroundTexture, uv + float2(1.0, 1.0) * depth) * 0.09;

                // Apply visual tint overlay and alpha blending
                fixed4 col = lerp(sum, _Color, _Color.a);
                col.a = i.color.a; // Keep UI.Image alpha multiplier
                
                return col;
            }
            ENDCG
        }
    }
}
