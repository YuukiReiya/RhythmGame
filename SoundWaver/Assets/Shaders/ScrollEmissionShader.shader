//スタンダードシェーダーのEmission + EmissionTextureのUV移動を用いたスクロールを予定
Shader "Custom/EmissionShader" {
	Properties{
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_MyEmissionColor("Emission Color",Color) = (0,0,0,0) //【1】
	}
		SubShader{
			Tags { "RenderType" = "Transparent" }

			CGPROGRAM
			#pragma surface surf Lambert

			float4 _Color;
			float4 _MyEmissionColor; //【2】

		struct Input { //【3】
			float2 Dummy; //【3】
		}; //【3】

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color; //【4】
			o.Emission = _MyEmissionColor; //【5】
		}
		ENDCG
	}
		FallBack "Diffuse"
}