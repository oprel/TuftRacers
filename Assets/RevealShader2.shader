Shader "Custom/RevealShader2" {
    Properties {
		[HideInInspector]_MainTex ("Secondary (RGB)", 2D) = "white" {}
		_Color2 ("Secondary Color", Color) = (1,1,1,1)
		[Header(Checkerboard)]
        _Color ("Background", Color) = (1,1,1,1)
		_LineWidth ("Line Width",  Range(0.01, 1)) = 0.01
    	_Density ("Density",  float)= .2
    	_Offset ("Offset Color", Range(0,1)) = 0.5
		[Toggle]_LocalWorld ("World Pos", Range(0,1)) = 0

        [Header(Dissolve)]
        _NoiseTex("Dissolve Noise", 2D) = "white"{} 
        _NScale ("Noise Scale", Range(0, 10)) = 1 
        _DisAmount("Noise Texture Opacity", Range(0.01, 1)) =0.01
        _Radius("Radius", Range(0, 1000)) = 0 
        _DisLineWidth("Line Width", Range(0, 2)) = 0 
        _DisLineColor("Line Tint", Color) = (1,1,1,1)   
		
    }
 
        SubShader{
            Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
            LOD 200 
            Blend SrcAlpha OneMinusSrcAlpha        
        
CGPROGRAM
 
#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0


 float3 _DissolvePosition; // from script

sampler2D _MainTex, _SecondTex;
float4 _Color, _Color2;
sampler2D _NoiseTex;
float _DisAmount, _NScale;
float _DisLineWidth;
float4 _DisLineColor;
float _Radius;
uniform float _LineWidth;
uniform float _Density;
uniform float _Offset;
uniform float _LocalWorld;
 
 
struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float3 worldPos;// built in value to use the world space position
	float3 worldNormal; // built in value for world normal
   
};
 
void surf (Input IN, inout SurfaceOutputStandard o) {
	//checkerboard
	fixed4 col = _Color;
	col = _Offset * (col - .5f) + 0.5f;
	float3 world= floor(IN.worldPos / _Density);
	float2 local = floor(IN.uv_MainTex * _Density);
	float2 c = lerp(local,world,_LocalWorld);
	float checker = frac(c.x/2 + c.y/2 + _LocalWorld* world.z/2) * 2;
	col = lerp(_Color, col, step(0.0,0.5-checker));
	float lx = step(_LineWidth, IN.uv_MainTex.x);
	float ly = step(_LineWidth, IN.uv_MainTex.y);
	float hx = step(IN.uv_MainTex.x, 1.0 - _LineWidth);
	float hy = step(IN.uv_MainTex.y, 1.0 - _LineWidth);
	col = lerp(_Color, col, lx*ly*hx*hy);
	half4 c2 = tex2D(_SecondTex, IN.uv_MainTex) * _Color2;

	// triplanar noise
	float3 blendNormal = saturate(pow(IN.worldNormal * 1.4,400));
    half4 nSide1 = tex2D(_NoiseTex, (IN.worldPos.xy + _Time.x) * _NScale); 
	half4 nSide2 = tex2D(_NoiseTex, (IN.worldPos.xz + _Time.x) * _NScale);
	half4 nTop = tex2D(_NoiseTex, (IN.worldPos.yz + _Time.x) * _NScale);

	float3 noisetexture = nSide1;
    noisetexture = lerp(noisetexture, nTop, blendNormal.x);
    noisetexture = lerp(noisetexture, nSide2, blendNormal.y);

	// distance influencer position to world position
	float3 dis = distance(_DissolvePosition, IN.worldPos);
	float3 sphere = 1 - saturate(dis / _Radius);
 
	float3 sphereNoise = noisetexture.r * sphere;

	float3 DissolveLine = step(sphereNoise - _DisLineWidth, _DisAmount) * step(_DisAmount,sphereNoise) ; // line between two textures
	DissolveLine *= _DisLineColor; // color the line
	
	float3 secondaryTex = (step(_DisAmount, sphereNoise) * col.rgb);
	float3 resultTex = secondaryTex + DissolveLine;
    float alpha = 1-step(resultTex,0);

    o.Albedo = resultTex;

	o.Emission = DissolveLine/2;
	o.Alpha = alpha;
   
}
ENDCG
 
    }
 
    Fallback "Standard"
}