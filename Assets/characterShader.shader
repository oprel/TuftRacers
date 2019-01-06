// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Character Indicator"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Indicator ("Indicator Color", Color) = (1,1,1,1)
		_Offset ("Offset", Float) = -1
    }
   
    SubShader
    {
        Tags { "Queue" = "Geometry+1" "RenderType" = "Opaque" }
 
		Pass
		{          
			Tags { "LightMode" = "Always" }
			ZWrite Off
			ZTest Greater
			Offset [_Offset], [_Offset]

			SetTexture [_MainTex]
			{
				constantColor [_Indicator]
				combine constant, texture
			}
		}
        CGPROGRAM
        #pragma surface surf BlinnPhong
       
        uniform float4 _Color;
        uniform float4 _Indicator;
        uniform sampler2D _MainTex;
         
        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };
       
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D ( _MainTex, IN.uv_MainTex).rgb * _Color;
        }
        ENDCG
 
//Pass: Fixed pipeline

       
    }
 
 
   
    Fallback " Glossy", 0
 
}