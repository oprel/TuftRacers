// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "oprel/finalretro" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _TexRes("Texture Resolution", Float) = 40
        _GeoDistort("Pixelsnap - Geo", Range(0,1)) = 0.5
        _GeoRes("Geometric Resolution", Float) = 40
        
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
			LOD 200

			Pass {
			Lighting On
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_fog
					#include "UnityCG.cginc"

					struct v2f
					{
						fixed4 pos : SV_POSITION;
						half4 color : COLOR0;
						half4 colorFog : COLOR1;
						float2 uv_MainTex : TEXCOORD0;
						half3 normal : TEXCOORD1;
						UNITY_FOG_COORDS(2)
					};

					float4 _MainTex_ST;
                    float _GeoRes;
                    float _TexRes;
                    float _GeoDistort;
					float4 _Color;

					v2f vert(appdata_full v)
					{
						v2f o;

                        float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                        wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

                        float4 sp = mul(UNITY_MATRIX_P, wp);
						//Vertex snapping
						float4 snapToPixel = UnityObjectToClipPos(v.vertex);
						float4 vertex = snapToPixel;
						vertex.xyz = snapToPixel.xyz / snapToPixel.w;
						vertex.x = floor(160 * vertex.x) / 160;
						vertex.y = floor(120 * vertex.y) / 120;
						vertex.xyz *= snapToPixel.w;
						o.pos = (1-_GeoDistort) * vertex + _GeoDistort * sp;

						//Vertex lighting 
						//o.color =  float4(ShadeVertexLights(v.vertex, v.normal), 1.0);
						o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
						//o.color *= v.color;

						float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

						//Affine Texture Mapping
						float4 affinePos = vertex; //vertex;				
						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.uv_MainTex *= distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
						o.normal = distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

						UNITY_TRANSFER_FOG(o,o.pos);
						return o;
					}

					sampler2D _MainTex;

					float4 frag(v2f IN) : COLOR
					{
                        half2 uv = half2((int)(IN.uv_MainTex.x * _TexRes/ IN.normal.r) / _TexRes, (int)(IN.uv_MainTex.y*_TexRes/ IN.normal.r) /_TexRes);	
						half4 color = tex2D(_MainTex, uv)*IN.color * _Color;
						UNITY_APPLY_FOG(IN.fogCoord, color);
						return color;
					}
				ENDCG
			}
	}
}