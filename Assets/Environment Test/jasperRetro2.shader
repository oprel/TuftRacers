Shader "oprel/psxretro" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		 _GeoRes("Geometric Resolution", Float) = 40
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 pos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _GeoRes;
		float4 MainTex_ST;
		

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			//USE WORLDPOS
			float4 wp = mul(UNITY_MATRIX_MV, IN.pos);
			wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

			float4 sp = mul(UNITY_MATRIX_P, wp);
			//o.position = sp;

			float2 uv = TRANSFORM_TEX(IN.uv_MainTex, MainTex);
			float3 a = float3(uv * sp.w, sp.w);
			uv = a.xy /a.z;
			//o.texcoord = float3(uv * sp.w, sp.w);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (MainTex, uv) * _Color;
			o.Albedo = c.rgb;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;


		}
		ENDCG
	}
	FallBack "Diffuse"
}
