// Unity 2017 Built-in pipeline reconstruction.
// Original _Roughness actually controls smoothness.
// Fixed constants and sqrt(specular) retain the supplied gamma-space BRDF.
Shader "LevelEditor/OC2_gloss_d_e_hl"
{
    Properties
    {
        _MaskColor ("Mask Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _DiffuseMap ("Diffuse Map", 2D) = "white" {}
        _Metallic ("Metallic", Float) = 0.04
        _Roughness ("Roughness (actually Smoothness)", Float) = 0.5
        _Brightness ("Brightness", Range(0,1)) = 0
        _highlight_colour ("highlight_colour", Color) = (0.9117647,0.8719588,0.7843858,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ColorMask RGB
        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf OC2 vertex:vert exclude_path:deferred exclude_path:prepass noambient nolightmap nodynlightmap novertexlights nofog
        #include "UnityPBSLighting.cginc"

        sampler2D _DiffuseMap;
        float4 _MaskColor, _highlight_colour;
        float _Metallic, _Roughness, _Brightness;

        struct Input
        {
            float2 uv_DiffuseMap;
            float3 worldPos;
            float3 worldNormal;
            float4 vertexColor;
        };
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexColor = v.color;
        }
        float Pow5OC2(float x)
        {
            float x2 = x * x;
            return x2 * x2 * x;
        }

        // Readable Disney diffuse / GGX equations from the supplied variant.
        half4 LightingOC2(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
        {
            float3 N = normalize(s.Normal);
            float3 V = normalize(viewDir);
            float3 L = gi.light.dir;
            float3 H = normalize(V + L);
            float nv = abs(dot(N, V));
            float nlRaw = max(dot(N, L), 0.0);
            float nl = saturate(nlRaw);
            float nh = saturate(dot(N, H));
            float lh = saturate(dot(L, H));

            float perceptualRoughness = 1.0 - s.Smoothness;
            float roughness = perceptualRoughness * perceptualRoughness;
            float roughness2 = roughness * roughness;
            float oneMinusReflectivity = 0.7790837 * (1.0 - s.Metallic);
            float3 diffuseColor = s.Albedo * oneMinusReflectivity;
            float3 specularColor = lerp(float3(0.2209163,0.2209163,0.2209163),
                                       s.Albedo, s.Metallic);

            float visibility = 0.5 / (nl * (nv * (1.0 - roughness) + roughness)
                                   + nv * (nl * (1.0 - roughness) + roughness) + 1e-5);
            float denominator = nh * nh * (roughness2 - 1.0) + 1.0;
            float distribution = roughness2 * 0.3183099
                               / (denominator * denominator + 1e-7);
            float specularTerm = sqrt(max(visibility * distribution * 3.141593, 1e-4)) * nl;
            specularTerm *= (dot(specularColor, specularColor) != 0.0) ? 1.0 : 0.0;
            float3 fresnel = lerp(specularColor, float3(1,1,1), Pow5OC2(1.0 - lh));

            float fd90MinusOne = 2.0 * lh * lh * perceptualRoughness - 0.5;
            float diffuseTerm = (1.0 + fd90MinusOne * Pow5OC2(1.0 - nlRaw))
                              * (1.0 + fd90MinusOne * Pow5OC2(1.0 - nv)) * nlRaw;
            float3 direct = gi.light.color * (diffuseColor * diffuseTerm + fresnel * specularTerm);

            float grazing = saturate(1.0 - oneMinusReflectivity + s.Smoothness);
            float3 indirectFresnel = lerp(specularColor, float3(grazing,grazing,grazing), Pow5OC2(1.0 - nv));
            float surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
            float3 reflection = gi.indirect.specular * indirectFresnel * surfaceReduction;
            // Surface compiler adds emission once in ForwardBase.
            return half4(direct + reflection, s.Alpha);
        }
        void LightingOC2_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
        {
            // Unity handles probe HDR decoding, box projection and blending.
            LightingStandard_GI(s, data, gi);
            gi.indirect.diffuse = 0;
        }
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 tex = tex2D(_DiffuseMap, IN.uv_DiffuseMap);
            float3 maskedEmission = tex.rgb * _MaskColor.rgb * tex.a * IN.vertexColor.a;
            o.Albedo = tex.rgb * IN.vertexColor.rgb + maskedEmission;
            o.Metallic = _Metallic;
            o.Smoothness = _Roughness;
            o.Occlusion = 1;
            o.Alpha = 1; // Texture alpha is a mask, not transparency.
            // Use the actual interpolated surface normal and camera direction.
            // No normal map is written, so Input.worldNormal is available directly.
            float3 N = normalize(IN.worldNormal);
            float3 V = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos);
            float rim = pow(1.0 - saturate(dot(N, V)), 0.9);
            o.Emission = maskedEmission + rim * _Brightness * _highlight_colour.rgb;
        }
        ENDCG
    }
    Fallback "Diffuse"
    // ShaderForgeMaterialInspector removed; Shader Forge is not required.
}