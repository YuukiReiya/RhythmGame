Shader "Unlit/Clip Blar Mask"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_SamplingDistance("Sampling Distance", float) = 1.0
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
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			float _SamplingDistance;
			static const int samplingCount = 7;
			static const half weights[samplingCount] = { 0.036, 0.113, 0.216, 0.269, 0.216, 0.113, 0.036 };

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
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert(appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				return tex2D(_MainTex, IN.texcoord) * IN.color;
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
					Offset -1, -1
				//ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMaterial AmbientAndDiffuse

				SetTexture[_MainTex]
				{
					Combine Texture * Primary
				}
			}
			}
}
