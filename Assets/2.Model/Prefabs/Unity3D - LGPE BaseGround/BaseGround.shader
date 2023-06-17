Shader "LGPE/BaseGround"
{
	Properties
	{
		_TexA("TexA", 2D) = "white" {}
		_TexB("TexB", 2D) = "white" {}
		_TexC("TexC", 2D) = "white" {}
		_TexD("TexD", 2D) = "white" {}
		_TexNoise("TexNoise", 2D) = "white" {}
		_TexTransition("TexTransition", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv_a : TEXCOORD0;
				float2 uv_b : TEXCOORD1;
				float2 uv_c : TEXCOORD2;
				float2 uv_d : TEXCOORD3;
				float2 uv_n : TEXCOORD4;
				float2 uv_t : TEXCOORD5;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _TexA;
			sampler2D _TexB;
			sampler2D _TexC;
			sampler2D _TexD;
			sampler2D _TexNoise;
			sampler2D _TexTransition;
			float4 _TexA_ST;
			float4 _TexB_ST;
			float4 _TexC_ST;
			float4 _TexD_ST;
			float4 _TexNoise_ST;
			float4 _TexTransition_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_a = TRANSFORM_TEX(v.uv, _TexA);
				o.uv_b = TRANSFORM_TEX(v.uv, _TexB);
				o.uv_c = TRANSFORM_TEX(v.uv, _TexC);
				o.uv_d = TRANSFORM_TEX(v.uv, _TexD);
				o.uv_n = TRANSFORM_TEX(v.uv, _TexNoise);
				o.uv_t = TRANSFORM_TEX(v.uv1, _TexTransition);
				return o;
			}
			
			fixed4 func(fixed4 a, fixed4 b, fixed4 c)
			{
				return a * (b - c) + c;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex_a = tex2D(_TexA, i.uv_a);
				fixed4 tex_b = tex2D(_TexB, i.uv_b);
				fixed4 tex_c = tex2D(_TexC, i.uv_c);
				fixed4 tex_d = tex2D(_TexD, i.uv_d);
				fixed4 tex_noise = tex2D(_TexNoise, i.uv_n);
				fixed4 tex_transition = tex2D(_TexTransition, i.uv_t);
				float trans_alpha = tex_transition.w;
				fixed4 col = tex_transition * func(trans_alpha,
					func(tex_noise, tex_a, tex_b),
					func(tex_noise, tex_c, tex_d));
				return col;
			}
			ENDCG
		}
	}
}
