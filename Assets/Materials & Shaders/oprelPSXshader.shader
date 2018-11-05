Shader "oprel/psx"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag 
            // 2.) This matches the "forward base" of the LightMode tag to ensure the shader compiles
            // properly for the forward bass pass. As with the LightMode tag, for any additional lights
            // this would be changed from _fwdbase to _fwdadd.
            #pragma multi_compile_fwdbase
 
            // 3.) Reference the Unity library that includes all the lighting shadow macros
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 texcoord : TEXCOORD;
                LIGHTING_COORDS(0,1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GeoRes;

            v2f vert(appdata_base v)
            {
                v2f o;

                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

                float4 sp = mul(UNITY_MATRIX_P, wp);
                o.position = sp;

                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord = float3(uv * sp.w, sp.w);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float attenuation = LIGHT_ATTENUATION(i);
                float2 uv = i.texcoord.xy / i.texcoord.z;
                return tex2D(_MainTex, uv) * _Color * 2 * attenuation;
            }

            ENDCG
        }
    }
      // 7.) To receive or cast a shadow, shaders must implement the appropriate "Shadow Collector" or "Shadow Caster" pass.
    // Although we haven't explicitly done so in this shader, if these passes are missing they will be read from a fallback
    // shader instead, so specify one here to import the collector/caster passes used in that fallback.
    Fallback "VertexLit"
}
