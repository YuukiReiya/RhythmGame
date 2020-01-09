Shader "Unlit/Clip Scroll Mask(Packed) (TextureClip)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                half2 worldPos : TEXCOORD1;
                half color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ClipTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half alpha = tex2D(_ClipTex, i.worldPos * 0.5 + float2(0.5, 0.5)).a;
                half4 mask = tex2D(_MainTex, i.uv);
                half4 mixed = saturate(ceil(i.color - 0.5));
                half4 col = saturate((mixed * 0.51 - i.color) / -0.49);

                col.a *= alpha;
                mask *= mixed;
                col.a *= mask.r + mask.g + mask.b + mask.a;
                return col;
            }
            ENDCG
        }
    }
        Fallback Off
}
