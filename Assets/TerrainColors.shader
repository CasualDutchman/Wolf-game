Shader "Terrain/ColorLerp"
{
	Properties
	{
		_MainTex ("Spring Texture", 2D) = "white" {}
		_MainTex2 ("Summer Texture", 2D) = "white" {}
		_MainTex3 ("Fall Texture", 2D) = "white" {}
		_MainTex4 ("Winter Texture", 2D) = "white" {}
		_Blend("Texture Blend", Range(0,4)) = 0.0
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard //fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _MainTex2;
			sampler2D _MainTex3;
			sampler2D _MainTex4;

			struct Input {
				float2 uv_MainTex;
				float2 uv_MainTex2;
				float2 uv_MainTex3;
				float2 uv_MainTex4;
			};

			half _Blend;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c;
				if (_Blend >= 0 && _Blend < 1) {
					c = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_MainTex2, IN.uv_MainTex2), _Blend);//tex0 -> tex1
				}
				else if (_Blend >= 1 && _Blend < 2) {
					c = lerp(tex2D(_MainTex2, IN.uv_MainTex2), tex2D(_MainTex3, IN.uv_MainTex3), _Blend - 1);//tex1 -> tex2
				}
				else if (_Blend >= 2 && _Blend < 3) {
					c = lerp(tex2D(_MainTex3, IN.uv_MainTex3), tex2D(_MainTex4, IN.uv_MainTex4), _Blend - 2);//tex2 -> tex3
				}
				else if (_Blend >= 3 && _Blend <= 4) {
					c = lerp(tex2D(_MainTex4, IN.uv_MainTex4), tex2D(_MainTex, IN.uv_MainTex), _Blend - 3);//tex3 -> tex0
				}

				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}
	FallBack "Diffuse"
}