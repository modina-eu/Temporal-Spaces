Shader "Unlit/DefaultPoseNet1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		//_PosTex("_PosTex", 2D) = "black0"{}
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
			float _resx2;
			float _resy2;
			float _time;
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
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
			float2 re = float2(16. / 9.,1.);
			float d1 = 0.;
			for (int j = 0; j < 17; j++)
			{
				if (_pos[j].z > 0.2) {
					d1 += smoothstep(0.01,0.004,distance(i.uv*re, _pos[j].xy / float2(_resx, _resy)*re));
				}
			}
		
			/*float p2 = 0.;
			for (int k = 0; k < 2; k++) {
				float t2 = _time -1.*k ;
				float2 u2 = float2(frac(t2 / _resx2), frac((t2) / (_resx2* _resy2)));
				 p2 += step(distance(i.uv*re, re*tex2D(_PosTex, u2).xy*float2(1., _resx / _resy) - float2(0., 0.)), 0.05)/(1.+k);
			}*/
			//float m2 = step(itn*frac((_time - (_resx - 1.)) / (_resx* itn)), mod(uv.y*_resy, itn));
			
			float itn = floor(_resy2 / 12.);
			float it = _resy2 / itn;
			float2 u0 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it );
			float2 u1 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 1. / it);
			float2 u2 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 2. / it);
			float2 u3 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 3. / it);
			float2 u4 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 4. / it);
			float2 u5 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 5. / it);
			float2 u6 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 6. / it);
			float2 u7 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 7. / it);
			float2 u8 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 8. / it);
			float2 u9 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 9. / it);
			float2 u10 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 10. / it);
			float2 u11 = float2(frac(_time / _resx2), frac(_time / _resx2 / itn) / it + 11. / it);
			float pp0 = tex2D(_PosTex, u0).xy;
			float pp1 = tex2D(_PosTex, u1).xy;
			float pp2 = tex2D(_PosTex, u2).xy;
			float pp3 = tex2D(_PosTex, u3).xy;
			float pp4 = tex2D(_PosTex, u4).xy;
			float pp5 = tex2D(_PosTex, u5).xy;
			float pp6 = tex2D(_PosTex, u6).xy;
			float pp7 = tex2D(_PosTex, u7).xy;
			float pp8 = tex2D(_PosTex, u8).xy;
			float pp9 = tex2D(_PosTex, u9).xy;
			float pp10 = tex2D(_PosTex, u10).xy;
			float pp11 = tex2D(_PosTex, u11).xy;
			float p = smoothstep(0.004,0.00,li(i.uv*re, ((tex2D(_PosTex, u0).xy-0.5)*_pr + tex2D(_PosTex, u7).xy )*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u1).xy - 0.5) *_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u2).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u3).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u4).xy - 0.5) *_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u5).xy - 0.5) *_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u6).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u8).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u9).xy - 0.5) *_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u10).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));
			p += smoothstep(0.004, 0.00, li(i.uv*re, ((tex2D(_PosTex, u11).xy - 0.5)*_pr + tex2D(_PosTex, u7).xy)*re, tex2D(_PosTex, u7).xy*re));

			float l1 = step(distance(i.uv.x, _pos1),0.001)+ step(distance(i.uv.x, _pos2a), 0.001)+ step(distance(i.uv.x, _pos3), 0.001);

                return lerp( col*0.5 +d1*0.4 ,float4(1.,0.,0.,1.), p)+l1*0.25;
            }
            ENDCG
        }
    }
}
