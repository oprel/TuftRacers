// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/fadingCenter" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,10)) = 0.5
		_Metallic ("Metallic", Range(0,3)) = 0.0
		_Distance ("Fade Distance", float) = 0.0
		_FadeSpeed ("Fade Speed", float) = 0.0
		_CentrePoint ("Centre", Vector) = (0, 0, 0, 0)
		[Header (Normals)]
		_NormalMap1 ("Normal Map 1", 2D) = "white" {}
		_normalMult1 ("Multiplier", Range(-1,1)) = 0.1
		_NormalMap2 ("Normal Map 2", 2D) = "white" {}
		_normalMult2 ("Multiplier", Range(-1,1)) = 0.1

	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _NormalMap1;
		sampler2D _NormalMap2;
		float4 _CentrePoint;
		float _FadeSpeed;

		struct Input {
			float2 uv_NormalMap1;
			float2 uv_NormalMap2;
            float3 worldPos;
		};


		half _Glossiness;
		half _Metallic;
		half _normalMult1;
		half _normalMult2;
		float _Distance;
		
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
	
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_NormalMap1, IN.uv_NormalMap1))*_normalMult1;
			o.Normal += UnpackNormal (tex2D (_NormalMap2, IN.uv_NormalMap2))*_normalMult2;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;;
			//o.Alpha = 1;

			float dist = distance(_CentrePoint.xyz, IN.worldPos);
			float alpha = saturate((_Distance-dist)/_FadeSpeed);
			o.Alpha = alpha * c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
