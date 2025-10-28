Shader "Custom/HeightGradient"
{
    Properties
    {
        _BottomColor ("Bottom (Grass/Moss)", Color) = (0.2, 0.45, 0.2, 1)
        _LowColor    ("Low (Dirt/Brown)",   Color) = (0.35, 0.25, 0.15, 1)
        _MidColor    ("Mid (Rock/Grey)",    Color) = (0.45, 0.45, 0.45, 1)
        _TopColor    ("Top (Snow/White)",   Color) = (0.95, 0.95, 0.98, 1)

        _MinHeight   ("Min World Height", Float) = 0
        _MaxHeight   ("Max World Height", Float) = 100
        _LowStart    ("Low Start (0-1)",  Range(0,1)) = 0.25
        _MidStart    ("Mid Start (0-1)",  Range(0,1)) = 0.5
        _TopStart    ("Top Start (0-1)",  Range(0,1)) = 0.75
        _Blend       ("Blend Width (0-0.5)", Range(0,0.5)) = 0.1

        _BrownPatchScale  ("Brown Patch Scale", Float) = 0.08
        _BrownStrength    ("Brown Strength",   Range(0,1)) = 0.6
        _BrownThreshold   ("Brown Threshold",  Range(0,1)) = 0.55
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #include "UnityCG.cginc"

        fixed4 _BottomColor, _LowColor, _MidColor, _TopColor;
        float  _MinHeight, _MaxHeight, _LowStart, _MidStart, _TopStart, _Blend;
        float  _BrownPatchScale, _BrownStrength, _BrownThreshold;

        struct Input
        {
            float3 worldPos;
        };

        // Simple hash-based 2D noise for brown patches
        float hash21(float2 p)
        {
            // cheap hash: range ~[0,1]
            p = frac(p * float2(123.34, 456.21));
            p += dot(p, p + 45.32);
            return frac(p.x * p.y);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Normalize height to 0..1 based on min/max world height
            float h = saturate((_MaxHeight == _MinHeight) ? 0.0 : (IN.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight));

            // Build soft weights for 4 bands with smoothstep blends
            float lowEdgeA  = _LowStart - _Blend;
            float lowEdgeB  = _LowStart + _Blend;
            float midEdgeA  = _MidStart - _Blend;
            float midEdgeB  = _MidStart + _Blend;
            float topEdgeA  = _TopStart - _Blend;
            float topEdgeB  = _TopStart + _Blend;

            // Bottom weight dominates below LowStart
            float wBottom = 1.0 - smoothstep(lowEdgeA, lowEdgeB, h);

            // Low band between LowStart and MidStart
            float wLow = smoothstep(lowEdgeA, lowEdgeB, h) * (1.0 - smoothstep(midEdgeA, midEdgeB, h));

            // Mid band between MidStart and TopStart
            float wMid = smoothstep(midEdgeA, midEdgeB, h) * (1.0 - smoothstep(topEdgeA, topEdgeB, h));

            // Top band above TopStart
            float wTop = smoothstep(topEdgeA, topEdgeB, h);

            // Normalize weights so they sum ~1
            float sumW = max(0.0001, (wBottom + wLow + wMid + wTop));
            wBottom /= sumW; wLow /= sumW; wMid /= sumW; wTop /= sumW;

            // Brown patch mask near the bottom using low-frequency noise in world XZ
            float2 nPos = IN.worldPos.xz * _BrownPatchScale;
            float noise = hash21(nPos);
            float brownMask = smoothstep(_BrownThreshold, 1.0, noise);

            // Mix grass with some brown splotches; stronger only at low elevations
            float bottomBrownFactor = brownMask * _BrownStrength * (1.0 - smoothstep(0.2, 0.6, h));
            fixed3 bottomTint = lerp(_BottomColor.rgb, _LowColor.rgb, bottomBrownFactor);

            fixed3 col = bottomTint * wBottom
                       + _LowColor.rgb * wLow
                       + _MidColor.rgb * wMid
                       + _TopColor.rgb * wTop;

            // Simple roughness/metal for terrain look
            o.Albedo = col;
            o.Metallic = 0.0;
            o.Smoothness = 0.35;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
