Shader "Custom/ULTF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Cutoff ("Cutoff", Range(0,1)) = 0.35
        _EdgeSmoothness ("Edge Smoothness", Range(0.001,0.1)) = 0.02

        // WIND
        _WindStrength ("Wind Strength", Range(0,5)) = 1
        _WindSpeed ("Wind Speed", Range(0,10)) = 2

        _LeafFlutter ("Leaf Flutter", Range(0,5)) = 1
        _LeafTwist ("Leaf Twist", Range(0,5)) = 1

        _BendStrength ("Bend Strength", Range(0,5)) = 1

        _NoiseScale ("Noise Scale", Range(0,10)) = 2

        // LIGHT
        _Ambient ("Ambient", Range(0,2)) = 0.5
        _BackLight ("BackLight", Range(0,5)) = 1.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }

        Cull Off
        ZWrite On
        AlphaToMask On

        Pass
        {
            Name "ForwardLit"

            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;

            half4 _Color;

            float _Cutoff;
            float _EdgeSmoothness;

            float _WindStrength;
            float _WindSpeed;

            float _LeafFlutter;
            float _LeafTwist;
            float _BendStrength;

            float _NoiseScale;

            float _Ambient;
            float _BackLight;
            
            // ROTATION

            float3 RotateAroundX(float3 pos, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);

                return float3(
                    pos.x,
                    pos.y * c - pos.z * s,
                    pos.y * s + pos.z * c
                );
            }

            float3 RotateAroundY(float3 pos, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);

                return float3(
                    pos.x * c + pos.z * s,
                    pos.y,
                    -pos.x * s + pos.z * c
                );
            }

            float hash(float2 p)
            {
                return frac(
                    sin(dot(p,float2(127.1,311.7)))
                    * 43758.5453
                );
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 pos = IN.positionOS.xyz;

                float time =
                    _Time.y * _WindSpeed;
                
                // BASE MASK
                // base fixe / haut mouvant

                float mask =
                    pow(IN.uv.y, 2.0);
                
                // UNIQUE RANDOM

                float rnd =
                    hash(pos.xy);
                
                // MAIN WIND

                float mainWind =
                    sin(
                        time +
                        rnd * 10
                    ) * _WindStrength;
                
                // FLUTTER

                float flutter =
                    sin(
                        time * 6 +
                        pos.x * 12 +
                        rnd * 5
                    ) * _LeafFlutter;

                
                // TWIST
                

                float twist =
                    cos(
                        time * 3 +
                        rnd * 8
                    ) * _LeafTwist;
                
                // BEND

                float bend =
                    mainWind *
                    _BendStrength *
                    mask;

                // pivot base
                pos.y -= 0.5;

                // bend
                pos =
                    RotateAroundX(
                        pos,
                        bend * 0.25
                    );

                // twist
                pos =
                    RotateAroundY(
                        pos,
                        twist * 0.1 * mask
                    );

                // flutter
                pos.x += flutter * 0.02 * mask;
                pos.z += flutter * 0.01 * mask;

                // remettre pivot
                pos.y += 0.5;

                VertexPositionInputs posInputs =
                    GetVertexPositionInputs(pos);

                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS =
                    posInputs.positionCS;

                OUT.positionWS =
                    posInputs.positionWS;

                OUT.normalWS =
                    normalInputs.normalWS;

                OUT.uv =
                    TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        IN.uv
                    );

                half4 col = tex * _Color;

                // SOFT CUTOUT
                float alpha =
                    smoothstep(
                        _Cutoff - _EdgeSmoothness,
                        _Cutoff + _EdgeSmoothness,
                        col.a
                    );

                clip(alpha - 0.01);

                // LIGHTING

                Light light =
                    GetMainLight();

                float3 normal =
                    normalize(IN.normalWS);

                // double sided foliage
                normal =
                    faceforward(
                        normal,
                        -normalize(_WorldSpaceCameraPos - IN.positionWS),
                        normal
                    );

                float NdotL =
                    saturate(
                        dot(normal, light.direction)
                    );

                NdotL =
                    NdotL * 0.5 + 0.5;

                float3 diffuse =
                    col.rgb *
                    light.color *
                    NdotL;

                float back =
                    pow(
                        saturate(
                            dot(-normal, light.direction)
                        ),
                        2
                    ) * _BackLight;

                float3 ambient =
                    col.rgb * _Ambient;

                float3 finalColor =
                    diffuse +
                    ambient +
                    back * col.rgb;

                return half4(finalColor, alpha);
            }

            ENDHLSL
        }
    }
}

/*Shader "UUTt"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    FallBack "UI/Default"
}*/