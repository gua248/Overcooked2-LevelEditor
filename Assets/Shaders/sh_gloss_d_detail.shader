Shader "Overcooked_2/OC2_gloss_d_detail" {
	Properties {
		_DiffuseMap ("Diffuse Map", 2D) = "white" {}
		_MaskColor ("Mask Color", Color) = (0.5019608,0.5019608,0.5019608,1)
		_Metallic ("Metallic", Float) = 0.04
		_Roughness ("Roughness", Float) = 0.5
		_DetailMap ("Detail Map", 2D) = "gray" {}
		_WorldUVMultipllier ("World UV Multipllier", Range(0, 10)) = 0
		_WorldUVRotator ("World UV Rotator", Range(0, 6.4)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "FORWARD"
			Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			GpuProgramID 678
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				float3 texcoord4 : TEXCOORD4;
				float3 texcoord5 : TEXCOORD5;
				float3 texcoord6 : TEXCOORD6;
				float4 color : COLOR0;
				float4 texcoord9 : TEXCOORD9;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float4 _LightColor0;
			float4 _MaskColor;
			float4 _DiffuseMap_ST;
			float _Metallic;
			float _Roughness;
			float4 _DetailMap_ST;
			float _WorldUVMultipllier;
			float _WorldUVRotator;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DiffuseMap;
			sampler2D _DetailMap;
			
			// Keywords: DIRECTIONAL LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                o.texcoord3 = unity_ObjectToWorld._m03_m13_m23_m33 * v.vertex.wwww + tmp0;
                tmp0 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp0 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp0;
                tmp0 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp0;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp0;
                o.texcoord.xy = v.texcoord.xy;
                o.texcoord1.xy = v.texcoord1.xy;
                o.texcoord2.xy = v.texcoord2.xy;
                tmp0.x = dot(v.normal.xyz, unity_WorldToObject._m00_m10_m20);
                tmp0.y = dot(v.normal.xyz, unity_WorldToObject._m01_m11_m21);
                tmp0.z = dot(v.normal.xyz, unity_WorldToObject._m02_m12_m22);
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                o.texcoord4.xyz = tmp0.xyz;
                tmp1.xyz = v.tangent.yyy * unity_ObjectToWorld._m01_m11_m21;
                tmp1.xyz = unity_ObjectToWorld._m00_m10_m20 * v.tangent.xxx + tmp1.xyz;
                tmp1.xyz = unity_ObjectToWorld._m02_m12_m22 * v.tangent.zzz + tmp1.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                o.texcoord5.xyz = tmp1.xyz;
                tmp2.xyz = tmp0.zxy * tmp1.yzx;
                tmp0.xyz = tmp0.yzx * tmp1.zxy + -tmp2.xyz;
                tmp0.xyz = tmp0.xyz * v.tangent.www;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                o.texcoord6.xyz = tmp0.www * tmp0.xyz;
                o.color = v.color;
                o.texcoord9 = float4(0.0, 0.0, 0.0, 0.0);
                return o;
			}
			// Keywords: DIRECTIONAL LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                float4 tmp6;
                float4 tmp7;
                float4 tmp8;
                float4 tmp9;
                float4 tmp10;
                tmp0.x = dot(inp.texcoord4.xyz, inp.texcoord4.xyz);
                tmp0.x = rsqrt(tmp0.x);
                tmp0.xyz = tmp0.xxx * inp.texcoord4.xyz;
                tmp1.xyz = _WorldSpaceCameraPos - inp.texcoord3.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp2.xyz = tmp0.www * tmp1.xyz;
                tmp1.w = dot(-tmp2.xyz, tmp0.xyz);
                tmp1.w = tmp1.w + tmp1.w;
                tmp3.xyz = tmp0.xyz * -tmp1.www + -tmp2.xyz;
                tmp1.w = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp4.xyz = tmp1.www * _WorldSpaceLightPos0.xyz;
                tmp1.xyz = tmp1.xyz * tmp0.www + tmp4.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp0.w = 1.0 - _Roughness;
                tmp1.w = tmp0.w * tmp0.w;
                tmp2.w = unity_SpecCube0_ProbePosition.w > 0.0;
                if (tmp2.w) {
                    tmp2.w = dot(tmp3.xyz, tmp3.xyz);
                    tmp2.w = rsqrt(tmp2.w);
                    tmp5.xyz = tmp2.www * tmp3.xyz;
                    tmp6.xyz = unity_SpecCube0_BoxMax.xyz - inp.texcoord3.xyz;
                    tmp6.xyz = tmp6.xyz / tmp5.xyz;
                    tmp7.xyz = unity_SpecCube0_BoxMin.xyz - inp.texcoord3.xyz;
                    tmp7.xyz = tmp7.xyz / tmp5.xyz;
                    tmp8.xyz = tmp5.xyz > float3(0.0, 0.0, 0.0);
                    tmp6.xyz = tmp8.xyz ? tmp6.xyz : tmp7.xyz;
                    tmp2.w = min(tmp6.y, tmp6.x);
                    tmp2.w = min(tmp6.z, tmp2.w);
                    tmp6.xyz = inp.texcoord3.xyz - unity_SpecCube0_ProbePosition.xyz;
                    tmp5.xyz = tmp5.xyz * tmp2.www + tmp6.xyz;
                } else {
                    tmp5.xyz = tmp3.xyz;
                }
                tmp2.w = -tmp0.w * 0.7 + 1.7;
                tmp2.w = tmp0.w * tmp2.w;
                tmp2.w = tmp2.w * 6.0;
                tmp5 = UNITY_SAMPLE_TEXCUBE_SAMPLER(unity_SpecCube0, unity_SpecCube0, float4(tmp5.xyz, tmp2.w));
                tmp3.w = tmp5.w - 1.0;
                tmp3.w = unity_SpecCube0_HDR.w * tmp3.w + 1.0;
                tmp3.w = tmp3.w * unity_SpecCube0_HDR.x;
                tmp6.xyz = tmp5.xyz * tmp3.www;
                tmp4.w = unity_SpecCube0_BoxMin.w < 0.99999;
                if (tmp4.w) {
                    tmp4.w = unity_SpecCube1_ProbePosition.w > 0.0;
                    if (tmp4.w) {
                        tmp4.w = dot(tmp3.xyz, tmp3.xyz);
                        tmp4.w = rsqrt(tmp4.w);
                        tmp7.xyz = tmp3.xyz * tmp4.www;
                        tmp8.xyz = unity_SpecCube1_BoxMax.xyz - inp.texcoord3.xyz;
                        tmp8.xyz = tmp8.xyz / tmp7.xyz;
                        tmp9.xyz = unity_SpecCube1_BoxMin.xyz - inp.texcoord3.xyz;
                        tmp9.xyz = tmp9.xyz / tmp7.xyz;
                        tmp10.xyz = tmp7.xyz > float3(0.0, 0.0, 0.0);
                        tmp8.xyz = tmp10.xyz ? tmp8.xyz : tmp9.xyz;
                        tmp4.w = min(tmp8.y, tmp8.x);
                        tmp4.w = min(tmp8.z, tmp4.w);
                        tmp8.xyz = inp.texcoord3.xyz - unity_SpecCube1_ProbePosition.xyz;
                        tmp3.xyz = tmp7.xyz * tmp4.www + tmp8.xyz;
                    }
                    tmp7 = UNITY_SAMPLE_TEXCUBE_SAMPLER(unity_SpecCube0, unity_SpecCube0, float4(tmp3.xyz, tmp2.w));
                    tmp2.w = tmp7.w - 1.0;
                    tmp2.w = unity_SpecCube1_HDR.w * tmp2.w + 1.0;
                    tmp2.w = tmp2.w * unity_SpecCube1_HDR.x;
                    tmp3.xyz = tmp7.xyz * tmp2.www;
                    tmp5.xyz = tmp3.www * tmp5.xyz + -tmp3.xyz;
                    tmp6.xyz = unity_SpecCube0_BoxMin.www * tmp5.xyz + tmp3.xyz;
                }
                tmp2.w = dot(tmp0.xyz, tmp4.xyz);
                tmp2.w = max(tmp2.w, 0.0);
                tmp3.x = min(tmp2.w, 1.0);
                tmp3.y = saturate(dot(tmp4.xyz, tmp1.xyz));
                tmp3.zw = inp.texcoord.xy * _DiffuseMap_ST.xy + _DiffuseMap_ST.zw;
                tmp4 = tex2D(_DiffuseMap, tmp3.zw);
                tmp5.xyz = _MaskColor.xyz > float3(0.5, 0.5, 0.5);
                tmp7.xyz = _MaskColor.xyz - float3(0.5, 0.5, 0.5);
                tmp7.xyz = -tmp7.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp8.xyz = float3(1.0, 1.0, 1.0) - tmp4.xyz;
                tmp7.xyz = -tmp7.xyz * tmp8.xyz + float3(1.0, 1.0, 1.0);
                tmp8.xyz = tmp4.xyz * _MaskColor.xyz;
                tmp8.xyz = tmp8.xyz + tmp8.xyz;
                tmp5.xyz = saturate(tmp5.xyz ? tmp7.xyz : tmp8.xyz);
                tmp4.xyz = tmp4.xyz - tmp5.xyz;
                tmp4.xyz = tmp4.www * tmp4.xyz + tmp5.xyz;
                tmp5.x = sin(_WorldUVRotator);
                tmp7.x = cos(_WorldUVRotator);
                tmp3.zw = _WorldUVMultipllier.xx * inp.texcoord3.xz + float2(-0.5, -0.5);
                tmp8.x = -tmp5.x;
                tmp8.y = tmp7.x;
                tmp8.z = tmp5.x;
                tmp5.x = dot(tmp3.xy, tmp8.xy);
                tmp5.y = dot(tmp3.xy, tmp8.xy);
                tmp3.zw = tmp5.xy + float2(0.5, 0.5);
                tmp3.zw = tmp3.zw * _DetailMap_ST.xy + _DetailMap_ST.zw;
                tmp5 = tex2D(_DetailMap, tmp3.zw);
                tmp7.xyz = tmp4.xyz > float3(0.5, 0.5, 0.5);
                tmp8.xyz = tmp4.xyz - float3(0.5, 0.5, 0.5);
                tmp8.xyz = -tmp8.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp9.xyz = float3(1.0, 1.0, 1.0) - tmp5.xyz;
                tmp8.xyz = -tmp8.xyz * tmp9.xyz + float3(1.0, 1.0, 1.0);
                tmp5.xyz = tmp4.xyz * tmp5.xyz;
                tmp5.xyz = tmp5.xyz + tmp5.xyz;
                tmp5.xyz = saturate(tmp7.xyz ? tmp8.xyz : tmp5.xyz);
                tmp5.xyz = tmp5.xyz - tmp4.xyz;
                tmp4.xyz = inp.color.www * tmp5.xyz + tmp4.xyz;
                tmp5.xyz = tmp4.xyz - float3(0.2209163, 0.2209163, 0.2209163);
                tmp5.xyz = _Metallic.xxx * tmp5.xyz + float3(0.2209163, 0.2209163, 0.2209163);
                tmp3.z = -_Metallic * 0.7790837 + 0.7790837;
                tmp4.xyz = tmp3.zzz * tmp4.xyz;
                tmp3.z = 1.0 - tmp3.z;
                tmp2.x = dot(tmp0.xyz, tmp2.xyz);
                tmp0.x = saturate(dot(tmp0.xyz, tmp1.xyz));
                tmp0.y = -tmp0.w * tmp0.w + 1.0;
                tmp0.z = abs(tmp2.x) * tmp0.y + tmp1.w;
                tmp0.y = tmp3.x * tmp0.y + tmp1.w;
                tmp0.y = tmp0.y * abs(tmp2.x);
                tmp0.y = tmp3.x * tmp0.z + tmp0.y;
                tmp0.y = tmp0.y + 0.00001;
                tmp0.y = 0.5 / tmp0.y;
                tmp0.z = tmp1.w * tmp1.w;
                tmp1.x = tmp0.x * tmp0.z + -tmp0.x;
                tmp0.x = tmp1.x * tmp0.x + 1.0;
                tmp0.z = tmp0.z * 0.3183099;
                tmp0.x = tmp0.x * tmp0.x + 0.0000001;
                tmp0.x = tmp0.z / tmp0.x;
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = tmp0.x * 3.141593;
                tmp0.x = max(tmp0.x, 0.0001);
                tmp0.x = sqrt(tmp0.x);
                tmp0.x = tmp3.x * tmp0.x;
                tmp0.y = tmp1.w * 0.28;
                tmp0.y = -tmp0.y * tmp0.w + 1.0;
                tmp0.z = dot(tmp5.xyz, tmp5.xyz);
                tmp0.z = tmp0.z != 0.0;
                tmp0.z = tmp0.z ? 1.0 : 0.0;
                tmp0.x = tmp0.z * tmp0.x;
                tmp1.xyz = tmp0.xxx * _LightColor0.xyz;
                tmp0.x = 1.0 - tmp3.y;
                tmp0.z = tmp0.x * tmp0.x;
                tmp0.z = tmp0.z * tmp0.z;
                tmp0.x = tmp0.x * tmp0.z;
                tmp7.xyz = float3(1.0, 1.0, 1.0) - tmp5.xyz;
                tmp7.xyz = tmp7.xyz * tmp0.xxx + tmp5.xyz;
                tmp0.x = saturate(tmp3.z + _Roughness);
                tmp0.z = 1.0 - abs(tmp2.x);
                tmp1.w = tmp0.z * tmp0.z;
                tmp1.w = tmp1.w * tmp1.w;
                tmp0.z = tmp0.z * tmp1.w;
                tmp2.xyz = tmp0.xxx - tmp5.xyz;
                tmp2.xyz = tmp0.zzz * tmp2.xyz + tmp5.xyz;
                tmp2.xyz = tmp2.xyz * tmp6.xyz;
                tmp2.xyz = tmp0.yyy * tmp2.xyz;
                tmp1.xyz = tmp1.xyz * tmp7.xyz + tmp2.xyz;
                tmp0.x = tmp3.y + tmp3.y;
                tmp0.x = tmp3.y * tmp0.x;
                tmp0.y = 1.0 - tmp2.w;
                tmp1.w = tmp0.y * tmp0.y;
                tmp1.w = tmp1.w * tmp1.w;
                tmp0.y = tmp0.y * tmp1.w;
                tmp0.x = tmp0.x * tmp0.w + -0.5;
                tmp0.y = tmp0.x * tmp0.y + 1.0;
                tmp0.x = tmp0.x * tmp0.z + 1.0;
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = tmp2.w * tmp0.x;
                tmp0.xyz = tmp0.xxx * _LightColor0.xyz;
                o.sv_target.xyz = tmp0.xyz * tmp4.xyz + tmp1.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Name "FORWARD_DELTA"
			Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			Blend One One, One One
			GpuProgramID 126857
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				float3 texcoord4 : TEXCOORD4;
				float3 texcoord5 : TEXCOORD5;
				float3 texcoord6 : TEXCOORD6;
				float4 color : COLOR0;
				float3 texcoord7 : TEXCOORD7;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4x4 unity_WorldToLight;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _LightColor0;
			float4 _MaskColor;
			float4 _DiffuseMap_ST;
			float _Metallic;
			float _Roughness;
			float4 _DetailMap_ST;
			float _WorldUVMultipllier;
			float _WorldUVRotator;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _LightTexture0;
			sampler2D _DiffuseMap;
			sampler2D _DetailMap;
			
			// Keywords: POINT LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0 = unity_ObjectToWorld._m03_m13_m23_m33 * v.vertex.wwww + tmp0;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy;
                o.texcoord1.xy = v.texcoord1.xy;
                o.texcoord2.xy = v.texcoord2.xy;
                o.texcoord3 = tmp0;
                tmp1.x = dot(v.normal.xyz, unity_WorldToObject._m00_m10_m20);
                tmp1.y = dot(v.normal.xyz, unity_WorldToObject._m01_m11_m21);
                tmp1.z = dot(v.normal.xyz, unity_WorldToObject._m02_m12_m22);
                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp1.xyz = tmp1.www * tmp1.xyz;
                o.texcoord4.xyz = tmp1.xyz;
                tmp2.xyz = v.tangent.yyy * unity_ObjectToWorld._m01_m11_m21;
                tmp2.xyz = unity_ObjectToWorld._m00_m10_m20 * v.tangent.xxx + tmp2.xyz;
                tmp2.xyz = unity_ObjectToWorld._m02_m12_m22 * v.tangent.zzz + tmp2.xyz;
                tmp1.w = dot(tmp2.xyz, tmp2.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp2.xyz = tmp1.www * tmp2.xyz;
                o.texcoord5.xyz = tmp2.xyz;
                tmp3.xyz = tmp1.zxy * tmp2.yzx;
                tmp1.xyz = tmp1.yzx * tmp2.zxy + -tmp3.xyz;
                tmp1.xyz = tmp1.xyz * v.tangent.www;
                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp1.w = rsqrt(tmp1.w);
                o.texcoord6.xyz = tmp1.www * tmp1.xyz;
                o.color = v.color;
                tmp1.xyz = tmp0.yyy * unity_WorldToLight._m01_m11_m21;
                tmp1.xyz = unity_WorldToLight._m00_m10_m20 * tmp0.xxx + tmp1.xyz;
                tmp0.xyz = unity_WorldToLight._m02_m12_m22 * tmp0.zzz + tmp1.xyz;
                o.texcoord7.xyz = unity_WorldToLight._m03_m13_m23 * tmp0.www + tmp0.xyz;
                return o;
			}
			// Keywords: POINT LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                tmp0.xy = _WorldUVMultipllier.xx * inp.texcoord3.xz + float2(-0.5, -0.5);
                tmp1.x = sin(_WorldUVRotator);
                tmp2.x = cos(_WorldUVRotator);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.y = dot(tmp0.xy, tmp3.xy);
                tmp1.x = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.xy + float2(0.5, 0.5);
                tmp0.xy = tmp0.xy * _DetailMap_ST.xy + _DetailMap_ST.zw;
                tmp0 = tex2D(_DetailMap, tmp0.xy);
                tmp1.xyz = float3(1.0, 1.0, 1.0) - tmp0.xyz;
                tmp2.xyz = _MaskColor.xyz > float3(0.5, 0.5, 0.5);
                tmp3.xyz = _MaskColor.xyz - float3(0.5, 0.5, 0.5);
                tmp3.xyz = -tmp3.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp4.xy = inp.texcoord.xy * _DiffuseMap_ST.xy + _DiffuseMap_ST.zw;
                tmp4 = tex2D(_DiffuseMap, tmp4.xy);
                tmp5.xyz = float3(1.0, 1.0, 1.0) - tmp4.xyz;
                tmp3.xyz = -tmp3.xyz * tmp5.xyz + float3(1.0, 1.0, 1.0);
                tmp5.xyz = tmp4.xyz * _MaskColor.xyz;
                tmp5.xyz = tmp5.xyz + tmp5.xyz;
                tmp2.xyz = saturate(tmp2.xyz ? tmp3.xyz : tmp5.xyz);
                tmp3.xyz = tmp4.xyz - tmp2.xyz;
                tmp2.xyz = tmp4.www * tmp3.xyz + tmp2.xyz;
                tmp3.xyz = tmp2.xyz - float3(0.5, 0.5, 0.5);
                tmp3.xyz = -tmp3.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp1.xyz = -tmp3.xyz * tmp1.xyz + float3(1.0, 1.0, 1.0);
                tmp0.xyz = tmp0.xyz * tmp2.xyz;
                tmp0.xyz = tmp0.xyz + tmp0.xyz;
                tmp3.xyz = tmp2.xyz > float3(0.5, 0.5, 0.5);
                tmp0.xyz = saturate(tmp3.xyz ? tmp1.xyz : tmp0.xyz);
                tmp0.xyz = tmp0.xyz - tmp2.xyz;
                tmp0.xyz = inp.color.www * tmp0.xyz + tmp2.xyz;
                tmp0.w = -_Metallic * 0.7790837 + 0.7790837;
                tmp1.xyz = tmp0.www * tmp0.xyz;
                tmp0.xyz = tmp0.xyz - float3(0.2209163, 0.2209163, 0.2209163);
                tmp0.xyz = _Metallic.xxx * tmp0.xyz + float3(0.2209163, 0.2209163, 0.2209163);
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = tmp0.w != 0.0;
                tmp0.w = tmp0.w ? 1.0 : 0.0;
                tmp2.xyz = _WorldSpaceCameraPos - inp.texcoord3.xyz;
                tmp1.w = dot(tmp2.xyz, tmp2.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp3.xyz = tmp1.www * tmp2.xyz;
                tmp2.w = dot(inp.texcoord4.xyz, inp.texcoord4.xyz);
                tmp2.w = rsqrt(tmp2.w);
                tmp4.xyz = tmp2.www * inp.texcoord4.xyz;
                tmp2.w = dot(tmp4.xyz, tmp3.xyz);
                tmp3.xyz = _WorldSpaceLightPos0.www * -inp.texcoord3.xyz + _WorldSpaceLightPos0.xyz;
                tmp3.w = dot(tmp3.xyz, tmp3.xyz);
                tmp3.w = rsqrt(tmp3.w);
                tmp3.xyz = tmp3.www * tmp3.xyz;
                tmp3.w = dot(tmp4.xyz, tmp3.xyz);
                tmp3.w = max(tmp3.w, 0.0);
                tmp4.w = min(tmp3.w, 1.0);
                tmp5.x = 1.0 - _Roughness;
                tmp5.y = -tmp5.x * tmp5.x + 1.0;
                tmp5.z = tmp5.x * tmp5.x;
                tmp5.w = tmp4.w * tmp5.y + tmp5.z;
                tmp5.y = abs(tmp2.w) * tmp5.y + tmp5.z;
                tmp5.z = tmp5.z * tmp5.z;
                tmp5.w = abs(tmp2.w) * tmp5.w;
                tmp2.w = 1.0 - abs(tmp2.w);
                tmp5.y = tmp4.w * tmp5.y + tmp5.w;
                tmp5.y = tmp5.y + 0.00001;
                tmp5.y = 0.5 / tmp5.y;
                tmp2.xyz = tmp2.xyz * tmp1.www + tmp3.xyz;
                tmp1.w = dot(tmp2.xyz, tmp2.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp2.xyz = tmp1.www * tmp2.xyz;
                tmp1.w = saturate(dot(tmp4.xyz, tmp2.xyz));
                tmp2.x = saturate(dot(tmp3.xyz, tmp2.xyz));
                tmp2.y = tmp1.w * tmp5.z + -tmp1.w;
                tmp1.w = tmp2.y * tmp1.w + 1.0;
                tmp1.w = tmp1.w * tmp1.w + 0.0000001;
                tmp2.y = tmp5.z * 0.3183099;
                tmp1.w = tmp2.y / tmp1.w;
                tmp1.w = tmp1.w * tmp5.y;
                tmp1.w = tmp1.w * 3.141593;
                tmp1.w = max(tmp1.w, 0.0001);
                tmp1.w = sqrt(tmp1.w);
                tmp1.w = tmp4.w * tmp1.w;
                tmp0.w = tmp0.w * tmp1.w;
                tmp1.w = dot(inp.texcoord7.xyz, inp.texcoord7.xyz);
                tmp4 = tex2D(_LightTexture0, tmp1.ww);
                tmp3.xyz = tmp4.xxx * _LightColor0.xyz;
                tmp4.xyz = tmp0.www * tmp3.xyz;
                tmp5.yzw = float3(1.0, 1.0, 1.0) - tmp0.xyz;
                tmp0.w = 1.0 - tmp2.x;
                tmp1.w = tmp0.w * tmp0.w;
                tmp1.w = tmp1.w * tmp1.w;
                tmp0.w = tmp0.w * tmp1.w;
                tmp0.xyz = tmp5.yzw * tmp0.www + tmp0.xyz;
                tmp0.xyz = tmp0.xyz * tmp4.xyz;
                tmp0.w = tmp2.w * tmp2.w;
                tmp0.w = tmp0.w * tmp0.w;
                tmp0.w = tmp2.w * tmp0.w;
                tmp1.w = tmp2.x + tmp2.x;
                tmp1.w = tmp2.x * tmp1.w;
                tmp1.w = tmp1.w * tmp5.x + -0.5;
                tmp0.w = tmp1.w * tmp0.w + 1.0;
                tmp2.x = 1.0 - tmp3.w;
                tmp2.y = tmp2.x * tmp2.x;
                tmp2.y = tmp2.y * tmp2.y;
                tmp2.x = tmp2.x * tmp2.y;
                tmp1.w = tmp1.w * tmp2.x + 1.0;
                tmp0.w = tmp0.w * tmp1.w;
                tmp0.w = tmp3.w * tmp0.w;
                tmp2.xyz = tmp3.xyz * tmp0.www;
                o.sv_target.xyz = tmp2.xyz * tmp1.xyz + tmp0.xyz;
                o.sv_target.w = 0.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Name "META"
			Tags { "LIGHTMODE" = "META" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			Cull Off
			GpuProgramID 150439
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				float4 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float unity_OneOverOutputBoost;
			float unity_MaxOutputValue;
			float4 _MaskColor;
			float4 _DiffuseMap_ST;
			float _Metallic;
			float _Roughness;
			float4 _DetailMap_ST;
			float _WorldUVMultipllier;
			float _WorldUVRotator;
			// Custom ConstantBuffers for Vertex Shader
			CBUFFER_START(UnityMetaPass)
				bool4 unity_MetaVertexControl;
			CBUFFER_END
			// Custom ConstantBuffers for Fragment Shader
			CBUFFER_START(UnityMetaPass)
				bool4 unity_MetaFragmentControl;
			CBUFFER_END
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DiffuseMap;
			sampler2D _DetailMap;
			
			// Keywords: SHADOWS_DEPTH LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0.x = v.vertex.z > 0.0;
                tmp0.z = tmp0.x ? 0.0001 : 0.0;
                tmp0.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                tmp0.xyz = unity_MetaVertexControl.xxx ? tmp0.xyz : v.vertex.xyz;
                tmp0.w = tmp0.z > 0.0;
                tmp1.z = tmp0.w ? 0.0001 : 0.0;
                tmp1.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                tmp0.xyz = unity_MetaVertexControl.yyy ? tmp1.xyz : tmp0.xyz;
                tmp1 = tmp0.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp1 = unity_ObjectToWorld._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.texcoord.xy = v.texcoord.xy;
                o.texcoord1.xy = v.texcoord1.xy;
                o.texcoord2.xy = v.texcoord2.xy;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                o.texcoord3 = unity_ObjectToWorld._m03_m13_m23_m33 * v.vertex.wwww + tmp0;
                o.color = v.color;
                return o;
			}
			// Keywords: SHADOWS_DEPTH LIGHTMAP_OFF DIRLIGHTMAP_OFF DYNAMICLIGHTMAP_OFF
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                tmp0.xy = _WorldUVMultipllier.xx * inp.texcoord3.xz + float2(-0.5, -0.5);
                tmp1.x = sin(_WorldUVRotator);
                tmp2.x = cos(_WorldUVRotator);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.y = dot(tmp0.xy, tmp3.xy);
                tmp1.x = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.xy + float2(0.5, 0.5);
                tmp0.xy = tmp0.xy * _DetailMap_ST.xy + _DetailMap_ST.zw;
                tmp0 = tex2D(_DetailMap, tmp0.xy);
                tmp1.xyz = float3(1.0, 1.0, 1.0) - tmp0.xyz;
                tmp2.xyz = _MaskColor.xyz > float3(0.5, 0.5, 0.5);
                tmp3.xyz = _MaskColor.xyz - float3(0.5, 0.5, 0.5);
                tmp3.xyz = -tmp3.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp4.xy = inp.texcoord.xy * _DiffuseMap_ST.xy + _DiffuseMap_ST.zw;
                tmp4 = tex2D(_DiffuseMap, tmp4.xy);
                tmp5.xyz = float3(1.0, 1.0, 1.0) - tmp4.xyz;
                tmp3.xyz = -tmp3.xyz * tmp5.xyz + float3(1.0, 1.0, 1.0);
                tmp5.xyz = tmp4.xyz * _MaskColor.xyz;
                tmp5.xyz = tmp5.xyz + tmp5.xyz;
                tmp2.xyz = saturate(tmp2.xyz ? tmp3.xyz : tmp5.xyz);
                tmp3.xyz = tmp4.xyz - tmp2.xyz;
                tmp2.xyz = tmp4.www * tmp3.xyz + tmp2.xyz;
                tmp3.xyz = tmp2.xyz - float3(0.5, 0.5, 0.5);
                tmp3.xyz = -tmp3.xyz * float3(2.0, 2.0, 2.0) + float3(1.0, 1.0, 1.0);
                tmp1.xyz = -tmp3.xyz * tmp1.xyz + float3(1.0, 1.0, 1.0);
                tmp0.xyz = tmp0.xyz * tmp2.xyz;
                tmp0.xyz = tmp0.xyz + tmp0.xyz;
                tmp3.xyz = tmp2.xyz > float3(0.5, 0.5, 0.5);
                tmp0.xyz = saturate(tmp3.xyz ? tmp1.xyz : tmp0.xyz);
                tmp0.xyz = tmp0.xyz - tmp2.xyz;
                tmp0.xyz = inp.color.www * tmp0.xyz + tmp2.xyz;
                tmp1.xyz = tmp0.xyz - float3(0.2209163, 0.2209163, 0.2209163);
                tmp1.xyz = _Metallic.xxx * tmp1.xyz + float3(0.2209163, 0.2209163, 0.2209163);
                tmp0.w = 1.0 - _Roughness;
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp1.xyz = tmp1.xyz * float3(0.5, 0.5, 0.5);
                tmp0.w = -_Metallic * 0.7790837 + 0.7790837;
                tmp0.xyz = tmp0.xyz * tmp0.www + tmp1.xyz;
                tmp0.xyz = log(tmp0.xyz);
                tmp0.w = saturate(unity_OneOverOutputBoost);
                tmp0.xyz = tmp0.xyz * tmp0.www;
                tmp0.xyz = exp(tmp0.xyz);
                tmp0.xyz = min(tmp0.xyz, unity_MaxOutputValue.xxx);
                tmp0.w = 1.0;
                tmp0 = unity_MetaFragmentControl ? tmp0 : float4(0.0, 0.0, 0.0, 0.0);
                o.sv_target = unity_MetaFragmentControl ? float4(0.0, 0.0, 0.0, 1.0) : tmp0;
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ShaderForgeMaterialInspector"
}