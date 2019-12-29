Shader "Unlit/Clip Blar Mask(Packed) (TextureClip)"
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
			float4 _MainTex_ST;
			sampler2D _ClipTex;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half color : COLOR;
				half2 texcoord : TEXCOORD0;
				half2 worldPos : TEXCOORD1;
			};


			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half alpha = tex2D(_ClipTex, IN.worldPos * 0.5 + float2(0.5, 0.5)).a;
				half4 mask = tex2D(_MainTex, IN.texcoord);
				half4 mixed = saturate(ceil(IN.color - 0.5));
				half4 col = saturate((mixed * 0.51 - IN.color) / -0.49);

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
