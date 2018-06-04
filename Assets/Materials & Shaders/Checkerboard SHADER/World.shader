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
			// Albedo comes from a texture tinted by color
		float4 answer;
			float lx = step(_LineWidth, IN.uv_MainTex2.x);
	        float ly = step(_LineWidth, IN.uv_MainTex2.y);
	        float hx = step(IN.uv_MainTex2.x, 1.0 - _LineWidth);
	        float hy = step(IN.uv_MainTex2.y, 1.0 - _LineWidth);

	        //lx = step(_LineWidth, IN.worldPos.x);
	        //ly = step(_LineWidth, IN.worldPos.y);
	        //hx = step(IN.worldPos.x, 1.0 - _LineWidth);
	        //hy = step(IN.worldPos.y, 1.0 - _LineWidth);


        float3 c = IN.worldPos * _Density;
        c = floor(c);

        //float size = max(c.w,c.h);
	    float checker = frac(c.x/2 + c.y/2 + c.z/2) * 2;

        fixed4 col = _Color;

        if (checker<0.5f){
			col -= 0.5f;
 			col*=_Offset;
 			col+=0.5f;

		}

        answer = lerp(_Color, col, lx*ly*hx*hy);




			//o.uv = o.uv * _Subdivisions;
			o.Albedo = answer;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
