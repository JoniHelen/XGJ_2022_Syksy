Shader "Unlit/ParticleCustomData"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowIntensity ("Glow Intensity", Range(1.0, 5.0)) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 customColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 customColor : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.customColor = v.customColor;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 imageCol = tex2D(_MainTex, i.uv);
                float4 outCol = i.customColor * _GlowIntensity;
                return float4(outCol.x, outCol.g, outCol.b, imageCol.a);
            }
            ENDCG
        }
    }
}
