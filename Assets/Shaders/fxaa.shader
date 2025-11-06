Shader "Hidden/Post FX/FXAA" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 34403
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _MainTex_TexelSize;
			float4 _ConsoleSettings;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.texcoord1.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.texcoord.xy = v.texcoord.xy;
                return o;
			}
			// Keywords: 
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
                tmp0 = _MainTex_TexelSize * float4(-0.5, -0.5, 0.5, 0.5) + inp.texcoord.xyxy;
                tmp1.xy = inp.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                tmp0 = tmp0 * _MainTex_ST + _MainTex_ST;
                tmp2 = tex2Dlod(_MainTex, float4(tmp0.xy, 0, 0.0));
                tmp3 = tex2Dlod(_MainTex, float4(tmp0.xw, 0, 0.0));
                tmp4 = tex2Dlod(_MainTex, float4(tmp0.zy, 0, 0.0));
                tmp0 = tex2Dlod(_MainTex, float4(tmp0.zw, 0, 0.0));
                tmp5 = tex2Dlod(_MainTex, float4(tmp1.xy, 0, 0.0));
                tmp0.x = max(tmp2.y, tmp3.y);
                tmp0.z = tmp4.y + 0.0026042;
                tmp0.w = min(tmp2.y, tmp3.y);
                tmp1.z = max(tmp0.y, tmp0.z);
                tmp1.w = min(tmp0.y, tmp0.z);
                tmp0.x = max(tmp0.x, tmp1.z);
                tmp0.w = min(tmp0.w, tmp1.w);
                tmp1.z = tmp0.x * _ConsoleSettings.z;
                tmp1.w = min(tmp5.y, tmp0.w);
                tmp1.z = max(tmp1.z, _ConsoleSettings.w);
                tmp2.x = max(tmp5.y, tmp0.x);
                tmp1.w = tmp2.x - tmp1.w;
                tmp1.z = tmp1.w >= tmp1.z;
                if (tmp1.z) {
                    tmp1.zw = _MainTex_TexelSize.xy * _ConsoleSettings.xx;
                    tmp2.xz = _MainTex_TexelSize.xy + _MainTex_TexelSize.xy;
                    tmp0.z = tmp3.y - tmp0.z;
                    tmp0.y = tmp0.y - tmp2.y;
                    tmp3.x = tmp0.y + tmp0.z;
                    tmp3.y = tmp0.z - tmp0.y;
                    tmp0.y = dot(tmp3.xy, tmp3.xy);
                    tmp0.y = rsqrt(tmp0.y);
                    tmp0.yz = tmp0.yy * tmp3.xy;
                    tmp2.yw = -tmp0.yz * tmp1.zw + tmp1.xy;
                    tmp3 = tex2Dlod(_MainTex, float4(tmp2.yw, 0, 0.0));
                    tmp1.zw = tmp0.yz * tmp1.zw + tmp1.xy;
                    tmp4 = tex2Dlod(_MainTex, float4(tmp1.zw, 0, 0.0));
                    tmp1.z = min(abs(tmp0.z), abs(tmp0.y));
                    tmp1.z = tmp1.z * _ConsoleSettings.y;
                    tmp0.yz = tmp0.yz / tmp1.zz;
                    tmp0.yz = max(tmp0.yz, float2(-2.0, -2.0));
                    tmp0.yz = min(tmp0.yz, float2(2.0, 2.0));
                    tmp1.zw = -tmp0.yz * tmp2.xz + tmp1.xy;
                    tmp6 = tex2Dlod(_MainTex, float4(tmp1.zw, 0, 0.0));
                    tmp0.yz = tmp0.yz * tmp2.xz + tmp1.xy;
                    tmp1 = tex2Dlod(_MainTex, float4(tmp0.yz, 0, 0.0));
                    tmp2.xyz = tmp3.xyz + tmp4.xyz;
                    tmp1.xyz = tmp1.xyz + tmp6.xyz;
                    tmp3.xyz = tmp2.xyz * float3(0.25, 0.25, 0.25);
                    tmp1.xyz = tmp1.xyz * float3(0.25, 0.25, 0.25) + tmp3.xyz;
                    tmp0.y = tmp1.y < tmp0.w;
                    tmp0.x = tmp0.x < tmp1.y;
                    tmp0.x = uint1(tmp0.x) | uint1(tmp0.y);
                    tmp0.yzw = tmp2.xyz * float3(0.5, 0.5, 0.5);
                    tmp5.xyz = tmp0.xxx ? tmp0.yzw : tmp1.xyz;
                }
                o.sv_target.xyz = tmp5.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}