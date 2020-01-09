Shader "Hidden/Unlit/Clip Scroll Mask(TextureClip)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _ScrollDirectionU("uv x scroll direction",Range(-1,1)) = 0
        _ScrollDirectionV("uv y scroll direction",Range(-1,1)) = 0
        _ScrollSpeedU("uv x scroll speed",Range(1.0,100.0)) = 10
        _ScrollSpeedV("uv y scroll speed",Range(1.0,100.0)) = 10

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
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ClipTex;
            float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
            half4 _MainTex_TexelSize;
            float _ScrollDirectionU;
            float _ScrollDirectionV;
            float _ScrollSpeedU;
            float _ScrollSpeedV;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //speed
                //※向きの補正のため加算ではなく減算をしている
                i.uv.x -= _ScrollDirectionU * _ScrollSpeedU * _Time;
                i.uv.y -= _ScrollDirectionV * _ScrollSpeedV * _Time;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
            Fallback "Unlit/Clip Scroll Mask"
}
