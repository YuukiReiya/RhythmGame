Shader "Unlit/LaneScroll"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_uvDirectionX("uv scroll direction x",Range(-1,1)) = 0
		_uvScrollSpeedX("uv scroll speed x",Range(0.0,100.0)) = 1
		_uvDirectionY("uv scroll direction y", Range(-1, 1)) = 0
		_uvScrollSpeedY("uv scroll speed y",Range(0.0,100.0)) = 1
		_color("main color",Color)=(1,1,1,1)
		_Pause("",Range(0,100))=0.1
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }

		//透過処理
		Tags	
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
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
			float _uvDirectionX;
			float _uvScrollSpeedX;
			float _uvDirectionY;
			float _uvScrollSpeedY;
			float4 _color;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				//speed * direction
				_uvDirectionX = _uvDirectionX * _uvScrollSpeedX;
				_uvDirectionY = _uvDirectionY * _uvScrollSpeedY;

				//add
				i.uv.x = i.uv.x + _uvDirectionX * _Time;
				i.uv.y = i.uv.y + _uvDirectionY * _Time;
				//調整中 以下
				//_Add = _Add + _Time * _Pause;
				//i.uv.x = i.uv.x + _uvDirectionX * _Add;
				//i.uv.y = i.uv.y + _uvDirectionY * _Add;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * _color;
            }
            ENDCG
        }
    }
}
