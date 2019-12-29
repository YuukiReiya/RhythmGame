
Shader "Hidden/Unlit/Clip Blar Mask (TextureClip)"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
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

			sampler2D _MainTex;
			sampler2D _ClipTex;
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float2 clipUV : TEXCOORD1;
				half4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert(appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.clipUV = (v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy) * 0.5 + float2(0.5, 0.5);
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
				col.a *= tex2D(_ClipTex, IN.clipUV).a;
				return col;
			}
			ENDCG
		}
	}
		Fallback "Unlit/Clip Blar Mask"
}
