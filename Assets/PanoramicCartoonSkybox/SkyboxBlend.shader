Shader "Custom/SkyboxBlend"
{
    Properties
    {
        _Tex1 ("Texture A", 2D) = "grey" {}
        _Tex2 ("Texture B", 2D) = "grey" {}
        _Blend ("Blend", Range(0,1)) = 0
        _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Tex1;
            sampler2D _Tex2;
            float _Blend;
            float _Exposure;
            float _Rotation;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            // Convertit une direction 3D en UV panoramique (equirectangular)
            float2 DirToUV(float3 dir, float rotation)
            {
                float rad = rotation * (UNITY_PI / 180.0);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float3 rotated = float3(
                    cosA * dir.x - sinA * dir.z,
                    dir.y,
                    sinA * dir.x + cosA * dir.z
                );

                float2 uv;
                uv.x = 0.5 + atan2(rotated.z, rotated.x) / (2.0 * UNITY_PI);
                uv.y = 0.5 + asin(clamp(rotated.y, -1.0, 1.0)) / UNITY_PI;
                return uv;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.texcoord);
                float2 uv = DirToUV(dir, _Rotation);

                half4 colA = tex2D(_Tex1, uv);
                half4 colB = tex2D(_Tex2, uv);

                half4 col = lerp(colA, colB, _Blend);
                col.rgb *= _Exposure * unity_ColorSpaceDouble.rgb;
                return col;
            }
            ENDCG
        }
    }
}