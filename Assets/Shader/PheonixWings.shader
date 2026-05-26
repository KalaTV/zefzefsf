Shader "Custom/PhoenixWings"
{
    Properties
    {
        _MainTex ("Flame Texture", 2D) = "white" {}

        _Color ("Main Color", Color) = (1,0.6,0.1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,0.8,1)
        
        // FLAME

        _DistortionStrength ("Distortion Strength", Range(0,1)) = 0.08
        _DistortionSpeed ("Distortion Speed", Range(0,10)) = 2

        _NoiseScale ("Noise Scale", Range(0,20)) = 5
        _NoiseSpeed ("Noise Speed", Range(0,10)) = 2

        _FlameIntensity ("Flame Intensity", Range(0,10)) = 4

        _DissolveAmount ("Dissolve", Range(0,1)) = 0.2
        _DissolveSoftness ("Dissolve Softness", Range(0.001,0.2)) = 0.05
        
        // WINGS MOTION

        _WingFlapAmplitude ("Wing Flap Amplitude", Range(0,5)) = 1
        _WingFlapSpeed ("Wing Flap Speed", Range(0,10)) = 2

        _FeatherFlutter ("Feather Flutter", Range(0,5)) = 1
        _FeatherWave ("Feather Wave", Range(0,5)) = 1

        _WingBend ("Wing Bend", Range(0,5)) = 1

        _TipMotion ("Tip Motion", Range(0,5)) = 2
        
        // LIGHTING

        _EmissionStrength ("Emission", Range(0,20)) = 6

        _Ambient ("Ambient", Range(0,2)) = 0.3

        _Smoothness ("Smoothness", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Off

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
            half4 _EdgeColor;

            float _DistortionStrength;
            float _DistortionSpeed;

            float _NoiseScale;
            float _NoiseSpeed;

            float _FlameIntensity;

            float _DissolveAmount;
            float _DissolveSoftness;

            float _WingFlapAmplitude;
            float _WingFlapSpeed;

            float _FeatherFlutter;
            float _FeatherWave;

            float _WingBend;
            float _TipMotion;

            float _EmissionStrength;
            float _Ambient;
            float _Smoothness;
            
            // NOISE

            float hash(float2 p)
            {
                return frac(
                    sin(dot(p,float2(127.1,311.7))) * 43758.5453123
                );
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a,b,u.x) + (c-a)*u.y*(1-u.x) + (d-b)*u.x*u.y;
            }
            
            // ROTATIONS

            float3 RotateZ(float3 p, float a)
            {
                float s = sin(a);
                float c = cos(a);

                return float3(p.x * c - p.y * s,p.x * s + p.y * c,p.z
                );
            }

            float3 RotateX(float3 p, float a)
            {
                float s = sin(a);
                float c = cos(a);

                return float3(p.x,p.y * c - p.z * s,p.y * s + p.z * c
                );
            }
            
            // VERTEX

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 pos = IN.positionOS.xyz;

                float time =_Time.y;
                
                // MASK

                float tipMask = pow(IN.uv.x, 2.0);

                float featherMask = pow(IN.uv.y, 1.5);
                
                // MAIN FLAP

                float flap =
                    sin(time * _WingFlapSpeed) * _WingFlapAmplitude;
                
                // FEATHER WAVE

                float wave =
                    sin(pos.x * 4 + time * 5) * _FeatherWave;
                
                // FLUTTER

                float flutter =
                    sin(pos.y * 12 + time * 14) * _FeatherFlutter;
                
                // TURBULENCE

                float n =
                    noise(pos.xy * 2 + time);
                
                // APPLY

                pos =
                    RotateZ(pos,flap * 0.12 * tipMask);

                pos =
                    RotateX(pos,wave * 0.08 * featherMask);

                pos.z += flutter * 0.03 * featherMask;

                pos.z += n * 0.05 * featherMask;

                pos.y += flap * 0.08 * featherMask;

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
                    TRANSFORM_TEX(
                        IN.uv,
                        _MainTex
                    );

                return OUT;
            }
            
            // FRAGMENT

            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y;
                
                // DISTORTION

                float2 distortionUV = IN.uv;

                float distortion =
                    noise(distortionUV * _NoiseScale + time * _DistortionSpeed
                    );

                distortionUV += (distortion - 0.5) * _DistortionStrength;
                
                // FLAME TEXTURE

                half4 tex =
                    SAMPLE_TEXTURE2D( _MainTex,sampler_MainTex,distortionUV
                    );
                
                // DISSOLVE

                float dissolve =
                    noise(distortionUV * (_NoiseScale * 1.5) - time * _NoiseSpeed
                    );

                float alpha =
                    smoothstep(_DissolveAmount, _DissolveAmount + _DissolveSoftness,dissolve
                    );
                
                // EDGE FIRE

                float edge =
                    smoothstep(0.4,1.0,dissolve
                    );

                float3 color =
                    lerp(_Color.rgb,_EdgeColor.rgb,edge
                    );
                
                // LIGHTING

                Light light =
                    GetMainLight();

                float3 normal =
                    normalize(IN.normalWS);

                normal =
                    faceforward(normal,-normalize(_WorldSpaceCameraPos - IN.positionWS), normal
                    );

                float NdotL =
                    saturate(dot(normal,light.direction)
                    );

                float3 diffuse =
                    color * (NdotL * 0.5 + 0.5);

                float3 emissive =
                    color * tex.rgb * _EmissionStrength;

                float3 ambient =
                    color * _Ambient;

                float3 finalColor = 
                    diffuse + emissive + ambient;

                return half4(
                    finalColor,
                    alpha * tex.a
                );
            }

            ENDHLSL
        }
    }
}