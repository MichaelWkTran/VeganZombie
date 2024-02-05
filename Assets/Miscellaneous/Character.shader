Shader "Unlit/Character"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
        _FlashAlpha ("Flash Alpha", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend One OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FlashAlpha;

            v2f vert (appdata _v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(_v.vertex);
                o.uv = TRANSFORM_TEX(_v.uv, _MainTex);
    
                return o;
            }

            fixed4 frag (v2f _i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, _i.uv);
                col.rgb = lerp(col.rgb, float3(1.0f, 1.0f, 1.0f), _FlashAlpha);
                col.rgb *= col.a;
    
                return col;
            }
            ENDCG
        }
    }
}
