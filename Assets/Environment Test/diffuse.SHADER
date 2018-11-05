// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced '_PPLAmbient' with 'UNITY_LIGHTMODEL_AMBIENT'

 Shader "ShaderNinja/Diffuse" {
 Properties {
   _MainTex ("_MainTex", 2D) = "" {}
 }
 #warning Upgrade NOTE: SubShader commented out; uses Unity 2.x per-pixel lighting. You should rewrite shader into a Surface Shader.
/*SubShader {
   Tags { "RenderType"="Opaque" }
   LOD 200

   Pass {
 CGPROGRAM

 #include "UnityCG.cginc"
 uniform sampler2D _MainTex;
 struct v2f {
     float2 uv_MainTex : TEXCOORD0;
 };
 struct f2l {
     half4 Albedo;
 };
 half4 frag (v2f i) : COLOR0 {
     f2l o;
     o.Albedo = tex2D(_MainTex,i.uv_MainTex);
     return o.Albedo * UNITY_LIGHTMODEL_AMBIENT * 2.0;
 }
 ENDCG
   }
   Pass {
 CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members uv_MainTex,normal,lightDir)
#pragma exclude_renderers d3d11
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_builtin

 #include "UnityCG.cginc"
 #include "AutoLight.cginc"
 struct v2f {
     float4 pos : SV_POSITION;
     LIGHTING_COORDS
     float2 uv_MainTex;
     float3 normal;
     float3 lightDir;
 };
 uniform float4 _MainTex_ST;
 v2f vert (appdata_tan v) {
     v2f o;
     o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
     o.normal = v.normal;
     o.lightDir = ObjSpaceLightDir(v.vertex);
     TRANSFER_VERTEX_TO_FRAGMENT(o);
     return o;
 }
 uniform sampler2D _MainTex;
 struct f2l {
     half4 Albedo;
     half3 Normal;
 };
 half4 frag (v2f i) : COLOR0 {
     f2l o;
     o.Normal = i.normal;
     o.Albedo = tex2D(_MainTex,i.uv_MainTex);
     return DiffuseLight (i.lightDir, o.Normal, o.Albedo, LIGHT_ATTENUATION(i));
 }
 ENDCG
   }
 }*/
 Fallback "VertexLit"
 }