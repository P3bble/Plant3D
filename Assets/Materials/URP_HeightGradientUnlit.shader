Shader "Custom/URP/HeightGradientSimple"
{
    Properties
    {
        _BottomColor ("Bottom (Grass/Moss)", Color) = (0.2, 0.45, 0.2, 1)
        _LowColor    ("Low (Dirt/Brown)",    Color) = (0.35, 0.25, 0.15, 1)
        _MidColor    ("Mid (Rock/Grey)",     Color) = (0.45, 0.45, 0.45, 1)
        _TopColor    ("Top (Snow/White)",    Color) = (0.95, 0.95, 0.98, 1)

        _MinHeight ("Min World Height", Float) = 0
        _MaxHeight ("Max World Height", Float) = 100
        _LowStart  ("Low Start (0-1)",  Range(0,1)) = 0.25
        _MidStart  ("Mid Start (0-1)",  Range(0,1)) = 0.5
        _TopStart  ("Top Start (0-1)",  Range(0,1)) = 0.75
        _Blend     ("Blend Width",      Range(0,0.5)) = 0.1

        _BrownPatchScale ("Brown Patch Scale", Float) = 0.08
        _BrownStrength   ("Brown Strength",   Range(0,1)) = 0.6
        _BrownThreshold  ("Brown Threshold",  Range(0,1)) = 0.55

        _CelSteps        ("Cel Steps", Range(2,6)) = 3
        _Ambient         ("Ambient Base", Range(0,1)) = 0.35
        _ShadowStrength  ("Shadow Strength", Range(0,1)) = 0.6
        _ShadowTint      ("Shadow Tint", Color) = (0,0,0,1)

        _RimColor     ("Rim Light Color", Color) = (1,1,1,1)
        _RimPower     ("Rim Power", Range(0.1, 8)) = 2.2
        _RimIntensity ("Rim Intensity", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _BottomColor, _LowColor, _MidColor, _TopColor;
            float  _MinHeight, _MaxHeight, _LowStart, _MidStart, _TopStart, _Blend;
            float  _BrownPatchScale, _BrownStrength, _BrownThreshold;
            float  _CelSteps, _Ambient, _ShadowStrength;
            float4 _ShadowTint;
            float4 _RimColor;
            float  _RimPower, _RimIntensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings o;
                float3 ws = TransformObjectToWorld(IN.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(ws);
                o.worldPos    = ws;
                o.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float Quantize(float x, float steps)
            {
                steps = max(2.0, steps);
                return floor(saturate(x) * steps) / (steps - 1.0);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float h = saturate((_MaxHeight == _MinHeight) ? 0.0 : (IN.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight));

                float lowA = _LowStart  - _Blend;
                float lowB = _LowStart  + _Blend;
                float midA = _MidStart  - _Blend;
                float midB = _MidStart  + _Blend;
                float topA = _TopStart  - _Blend;
                float topB = _TopStart  + _Blend;

                float wBottom = 1.0 - smoothstep(lowA, lowB, h);
                float wLow    = smoothstep(lowA, lowB, h) * (1.0 - smoothstep(midA, midB, h));
                float wMid    = smoothstep(midA, midB, h) * (1.0 - smoothstep(topA, topB, h));
                float wTop    = smoothstep(topA, topB, h);

                float sumW = max(0.0001, wBottom + wLow + wMid + wTop);
                wBottom /= sumW;
                wLow    /= sumW;
                wMid    /= sumW;
                wTop    /= sumW;

                float2 nPos = IN.worldPos.xz * _BrownPatchScale;
                float noise = hash21(nPos);
                float brownMask = smoothstep(_BrownThreshold, 1.0, noise);
                float bottomBrownFactor = brownMask * _BrownStrength * (1.0 - smoothstep(0.2, 0.6, h));
                float3 bottomTint = lerp(_BottomColor.rgb, _LowColor.rgb, bottomBrownFactor);

                float3 albedo = bottomTint      * wBottom +
                                _LowColor.rgb   * wLow    +
                                _MidColor.rgb   * wMid    +
                                _TopColor.rgb   * wTop;

                float3 N = normalize(IN.worldNormal);
                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);
                float  NdotL = saturate(dot(N, -L));

                float  cel = Quantize(NdotL, _CelSteps);
                float  shadowAtten = MainLightRealtimeShadow(TransformWorldToShadowCoord(IN.worldPos));
                float  litBand = lerp(cel * (1.0 - _ShadowStrength), cel, shadowAtten);

                float3 color = albedo * (_Ambient + litBand);
                color = lerp(color, color * _ShadowTint.rgb, (1.0 - shadowAtten) * _ShadowStrength);

                float3 V = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos);
                float rim = pow(1.0 - saturate(dot(V, N)), _RimPower);
                color += _RimColor.rgb * rim * _RimIntensity;

                return half4(color, 1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
