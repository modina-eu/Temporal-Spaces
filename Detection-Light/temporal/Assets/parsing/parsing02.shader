Shader "Unlit/parsing02"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_a0("_a0",Float) = 0
		_b0("_b0",Float) = 0
		_c0("_c0",Float) = 0
		_d0("_d0",Float) = 0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _a0;
			float _b0;
			float _c0;
			float _d0;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			float map(float value, float low1, float high1, float low2, float high2) {
				return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
			}
			float li(float2 uv, float2  a, float2 b) {
				float2 ua = uv - a; float2 ba = b - a;
				float h = clamp(dot(ua, ba) / dot(ba, ba), 0., 1.);
				return length(ua - ba * h);
			}
			fixed4 frag(v2f i) : SV_Target
			{
			float d1 = _a0 + _c0 * .75;
			float d2 = _a0 - _c0 * 0.75;
			float d3 = _b0 - _d0 * 0.75;
			float d4 = _b0 + _d0 * 0.75;
			float ra = (_c0 / _d0);
			float ra2 = (_d0 / _c0);

			float2 p0 = float2(_a0, _b0);
			float2 p1 = p0+float2(_c0, _d0)*0.5;
			float2 p2 = p0+float2(_c0, -_d0)*0.5;
			float2 p3 = p0+float2(-_c0, _d0)*0.5;
			float2 p4 = p0+float2(-_c0, -_d0)*0.5;

			float l1 = smoothstep(0.001, 0., li(i.uv, p1, p2));
			l1 += smoothstep(0.001, 0., li(i.uv, p3, p4));
			l1 += smoothstep(0.001, 0., li(i.uv, p1, p3));
			l1 += smoothstep(0.001, 0., li(i.uv, p4, p2));
				fixed4 col = tex2D(_MainTex,i.uv )+l1;
				return col;
			}
			ENDCG
		}
	}
}
