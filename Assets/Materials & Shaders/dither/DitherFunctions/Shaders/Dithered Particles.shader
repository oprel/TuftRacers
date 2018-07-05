// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dithered Transparent/Dithered Particles"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _DitherScale("Dither Scale", Float) = 10
        [NoScaleOffset]_DitherTex ("Dither Texture", 2D) = "white" {}
    }

    SubShader
    {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off
        //Blend One One
        Pass
        {            
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Dither Functions.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            uniform fixed4 _LightColor0;

            float4 _Color;
            float4 _MainTex_ST;         // For the Main Tex UV transform
            sampler2D _MainTex;         // Texture used for the line
            
            float _DitherScale;
            sampler2D _DitherTex;

            struct v2f
            {
                float4 pos      : POSITION;
                float4 col      : COLOR;
                float2 uv       : TEXCOORD0;
                float4 spos     : TEXCOORD1;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.col=v.color;
                o.spos = ComputeScreenPos(o.pos);

                return o;
            }

            float4 frag(v2f i) : COLOR
            { 
                float4 col = _Color * tex2D(_MainTex, i.uv);
                
                ditherClip(i.spos.xy / i.spos.w, i.col.a, _DitherTex, _DitherScale);
                i.col.a=1;
                return col * i.col;

            }

            ENDCG
        }
    }

}
