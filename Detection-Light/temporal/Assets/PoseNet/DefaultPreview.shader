Shader "Unlit/DefaultPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_PosTex("_PosTex", 2D) = "black"{}
		//_Pos("_pos", Vector[]) = (0,0,0,0)
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
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _PosTex;
			float3 _pos[17];
			float3 _pos2[17];
			float _resx;
			float _resy;
			//float _resx2;
			//float _resy2;
			//float _time;
			float _pr;
			float _pos1;
			float _pos2a;
			float _pos3;
			float _pp;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
			float map(float value, float min1, float max1, float min2, float max2) {
				return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
			}
			float li(float2 u, float2 a, float2 b) { float2 ua = u - a; float2 ba = b - a; float h = clamp(dot(ua, ba) / dot(ba, ba), 0., 1.);
			return length(ua - ba * h);
			}
			float box(float2 p, float2 b) {
				float2 q = abs(p) - b;
				return length(max(q, float2(0., 0.))) + min(0., max(q.x, q.y));
			}
			float2 tc(float _time, float _resx2, float v, float itn, float it) {
				return float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it+v/it);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
			float2 re = float2(16. / 9.,1.);

			i.uv *= 0.65;
			i.uv += float2(.25, 0);
			//i.uv *= float2(0.3,0.5);
			//i.uv += float2(0.4, 0.);
			float _time = _Time.y*10.;
			float _resx2 = 64.;
			float _resy2 = 64.;
			float _pr = 1.;
			float itn = floor(_resy2 / 12.);
			float it = _resy2 / itn;
			float2 u0 = tc(_time , _resx2,0. ,itn,it);
			float2 u1 = tc(_time, _resx2, 1., itn, it);
			float2 u2 = tc(_time, _resx2, 2., itn, it);
			float2 u3 = tc(_time, _resx2, 3., itn, it);
			float2 u4 = tc(_time, _resx2, 4., itn, it);
			float2 u5 = tc(_time, _resx2, 5., itn, it);
			float2 u6 = tc(_time, _resx2, 6., itn, it);
			float2 u7 = tc(_time, _resx2, 7., itn, it);
			float2 u8 = tc(_time, _resx2, 8., itn, it);
			float2 u9 = tc(_time, _resx2, 9., itn, it);
			float2 u10 = tc(_time, _resx2, 10., itn, it);
			float2 u11 = tc(_time, _resx2, 11., itn, it);

			float2 ub0 = tc(_time+1., _resx2, 0., itn, it);
			float2 ub1 = tc(_time + 1., _resx2, 1., itn, it);
			float2 ub2 = tc(_time + 1., _resx2, 2., itn, it);
			float2 ub3 = tc(_time + 1., _resx2, 3., itn, it);
			float2 ub4 = tc(_time + 1., _resx2, 4., itn, it);
			float2 ub5 = tc(_time + 1., _resx2, 5., itn, it);
			float2 ub6 = tc(_time + 1., _resx2, 6., itn, it);
			float2 ub7 = tc(_time + 1., _resx2, 7., itn, it);
			float2 ub8 = tc(_time + 1., _resx2, 8., itn, it);
			float2 ub9 = tc(_time + 1., _resx2, 9., itn, it);
			float2 ub10 = tc(_time + 1., _resx2, 10., itn, it);
			float2 ub11 = tc(_time + 1., _resx2, 11., itn, it);
			float a = 0.5;
			float ta = smoothstep(0., 1., frac(_time));
			float2 tp = lerp(tex2D(_PosTex, u7).xy, tex2D(_PosTex, ub7).xy, ta);
			float2 p1 = ((lerp(tex2D(_PosTex, u0).xy,tex2D(_PosTex, ub0).xy,ta) - a)*_pr + tp);
			float2 p2 = ((lerp(tex2D(_PosTex, u1).xy, tex2D(_PosTex, ub1).xy, ta) - a)*_pr + tp);
			float2 p3 = ((lerp(tex2D(_PosTex, u2).xy, tex2D(_PosTex, ub2).xy, ta) - a)*_pr + tp);
			float2 p4 = ((lerp(tex2D(_PosTex, u3).xy, tex2D(_PosTex, ub3).xy, ta) - a)*_pr + tp);
			float2 p5 = ((lerp(tex2D(_PosTex, u4).xy, tex2D(_PosTex, ub4).xy, ta) - a)*_pr + tp);
			float2 p6 = ((lerp(tex2D(_PosTex, u5).xy, tex2D(_PosTex, ub5).xy, ta) - a)*_pr + tp);
			float2 p7 = ((lerp(tex2D(_PosTex, u6).xy, tex2D(_PosTex, ub6).xy, ta) - a)*_pr + tp);
			float2 p8 = ((lerp(tex2D(_PosTex, u8).xy, tex2D(_PosTex, ub8).xy, ta) - a)*_pr + tp);
			float2 p9 = ((lerp(tex2D(_PosTex, u9).xy, tex2D(_PosTex, ub9).xy, ta) - a)*_pr + tp);
			float2 p10 = ((lerp(tex2D(_PosTex, u10).xy, tex2D(_PosTex, ub10).xy, ta) - a)*_pr + tp);
			float2 p11 = ((lerp(tex2D(_PosTex, u11).xy, tex2D(_PosTex, ub11).xy, ta) - a)*_pr + tp);
			float ts = 0.0015;
			float p = smoothstep(ts,0.00,li(i.uv*re, p1*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p2*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p3*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p4*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p5*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p6*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p7*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p8*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p9*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p10*re, tp*re));
			p += smoothstep(ts, 0.00, li(i.uv*re, p11*re, tp*re));

			float c = smoothstep(ts, 0.00, li(i.uv*re, p1*re, tp *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p2*re, p3 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p2*re, p4 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p4*re, p6 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p3*re, p5 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p5*re, p7 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, tp*re, p8 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, tp*re, p9 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p8*re, p10 *re));
			c += smoothstep(ts, 0.00, li(i.uv*re, p9*re, p11 *re));
            return lerp( col*0.5,float4(1.,0.,0.,1.), p)+c;
            }
            ENDCG
        }
    }
}
