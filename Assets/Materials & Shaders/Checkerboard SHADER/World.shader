Shader "Checkerboard/World" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_LineWidth ("Line Width", float) = 0
    	_Density ("Density", float) = 8
    	_Offset ("Offset Color", Range(0,1)) = 0.5
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0


	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex2 : TEXCOORD0;
			float3 worldPos;
			float4 screenPos;

		};

		half _Glossiness;
		half _Metallic;
		int _Subdivisions;
		uniform float4 _Color;
      uniform float _LineWidth;
      uniform float _Density;
      uniform float _Offset;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 col = _Color;
			col = _Offset * (col - .5f) + 0.5f;
			float3 c = floor(IN.worldPos * _Density);
			float checker = frac(c.x/2 + c.y/2 + c.z/2) * 2;
			col = lerp(_Color, col, step(0.0,0.5-checker));
			float lx = step(_LineWidth, IN.uv_MainTex2.x);
	        float ly = step(_LineWidth, IN.uv_MainTex2.y);
	        float hx = step(IN.uv_MainTex2.x, 1.0 - _LineWidth);
	        float hy = step(IN.uv_MainTex2.y, 1.0 - _LineWidth);
			col = lerp(_Color, col, lx*ly*hx*hy);
			
			o.Albedo = col;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
