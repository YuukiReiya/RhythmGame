//参考1:https://qiita.com/Nekomasu/items/d50a4409e48ad77bc7f2#%E9%80%8F%E9%81%8E%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6
//参考2:https://docs.unity3d.com/ja/current/Manual/SL-VertexFragmentShaderExamples.html
Shader "Unlit/SimpleScroll"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _ScrollDirectionU("uv x scroll direction",Range(-1,1))=0
        _ScrollDirectionV("uv y scroll direction",Range(-1,1))=0
        _ScrollSpeedU("uv x scroll speed",Range(1.0,100.0)) = 10
        _ScrollSpeedV("uv y scroll speed",Range(1.0,100.0)) = 10
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent"
            "RenderType"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                //頂点座標
                float4 vertex : POSITION;

                //UV座標
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ScrollDirectionU;
            float _ScrollDirectionV;
            float _ScrollSpeedU;
            float _ScrollSpeedV;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //  UV座標の決定
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //speed
                //※向きの補正のため加算ではなく減算をしている
                i.uv.x-=_ScrollDirectionU*_ScrollSpeedU*_Time;
                i.uv.y-=_ScrollDirectionV*_ScrollSpeedV*_Time;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
