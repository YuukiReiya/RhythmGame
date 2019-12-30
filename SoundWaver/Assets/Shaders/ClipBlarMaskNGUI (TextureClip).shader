//参考:http://albatrus.com/main/unity/8392
Shader "Hidden/Unlit/Clip Blar Mask (TextureClip)"
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
			sampler2D _ClipTex;
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
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
				float2 clipUV : TEXCOORD1;
				half2 coordV : TEXCOORD2;
				half2 coordH : TEXCOORD3;
				half2 offsetV: TEXCOORD4;
				half2 offsetH: TEXCOORD5;

				half4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert(appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				half2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				// サンプリングポイントのオフセット
				o.offsetV = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _SamplingDistance;
				o.offsetH = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _SamplingDistance;

				// サンプリング開始ポイントのUV座標
				o.coordV = uv - o.offsetV * ((samplingCount - 1) * 0.5);
				o.coordH = uv - o.offsetH * ((samplingCount - 1) * 0.5);

				o.color = v.color;
				o.texcoord = v.texcoord;
				o.clipUV = (v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy) * 0.5 + float2(0.5, 0.5);
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;

				// 垂直方向
				for (int j = 0; j < samplingCount; j++) {
					// サンプリングして重みを掛ける。後で水平方向も合成するため0.5をかける
					col += tex2D(_MainTex, IN.coordV) * weights[j] * 0.5;
					// offset分だけサンプリングポイントをずらす
					IN.coordV += IN.offsetV;
				}

				// 水平方向
				for (int j = 0; j < samplingCount; j++) {
					col += tex2D(_MainTex, IN.coordH) * weights[j] * 0.5;
					IN.coordH += IN.offsetH;
				}

				col.a *= tex2D(_ClipTex, IN.clipUV).a;
				return col;
			}
			ENDCG
		}
	}
		Fallback "Unlit/Clip Blar Mask"
}
