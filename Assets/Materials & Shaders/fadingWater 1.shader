// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaDependingDistance"
{
    Properties
    {
        _Radius ("Radius", Range(0.001, 5000)) = 10
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[Header (Normals)]
		_NormalMap1 ("Normal Map 1", 2D) = "white" {}
		_normalMult1 ("Multiplier", Range(-1,1)) = 0.1
		_NormalMap2 ("Normal Map 2", 2D) = "white" {}
		_normalMult2 ("Multiplier", Range(-1,1)) = 0.1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
 
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };
 

			half _Glossiness;
			half _Metallic;
			half _normalMult1;
			half _normalMult2;
			fixed4 _Color;
			float _Radius;
 
            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
 
            
 
            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = _Color;
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                col.a = 1-saturate(dist / _Radius);
                return col;
            }
 
            ENDCG
        }
    }
}