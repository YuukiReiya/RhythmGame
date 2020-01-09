Shader "Hidden/Unlit/Clip Scroll Mask 1"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

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
            "DisableBatching" = "True"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Offset -1, -1
            Fog { Mode Off }
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
            float2 _ClipArgs0 = float2(1000.0, 1000.0);
            float _ScrollDirectionU;
            float _ScrollDirectionV;
            float _ScrollSpeedU;
            float _ScrollSpeedV;

            v2f o;
            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Softness factor
                float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;

                //speed
                //※向きの補正のため加算ではなく減算をしている
                i.uv.x -= _ScrollDirectionU * _ScrollSpeedU * _Time;
                i.uv.y -= _ScrollDirectionV * _ScrollSpeedV * _Time;

                // Sample the texture
                half4 col = tex2D(_MainTex, i.uv) * i.color;
                col.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
                //return half4(1,0,0,1);
                return col;
            }
            ENDCG
        }
    }

     SubShader
     {
         LOD 100

         Tags
         {
             "Queue" = "Transparent"
             "IgnoreProjector" = "True"
             "RenderType" = "Transparent"
             "DisableBatching" = "True"
         }

         Pass
         {
             Cull Off
             Lighting Off
             ZWrite Off
             Fog { Mode Off }
             Blend SrcAlpha OneMinusSrcAlpha
             ColorMaterial AmbientAndDiffuse

             SetTexture[_MainTex]
             {
                 Combine Texture * Primary
             }
         }
     }
}
