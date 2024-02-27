Shader "Unlit/webcam"
{
    Properties
    {

	_MainTex4("Texture4", 2D) = "black" {}
	_Date("_Date",2D) = "black"{}
	_bl("_bl", 2D) = "black" {}
	_gui1("_gui1",2D) = "black"{}
	//_gui2("_gui2",2D) = "black"{}
	  _float1("_float1",Float) = 0
		_float2("_float2",Float) = 0
		_float3("_float3",Float) = 0
		  _float4("_float4",Float) = 0
		  _blur("_blur",Float) = 0
		  _step0to1("_step0to1", Range(0,1)) = 0
	    _step1to2("_step1to2", Range(0,1)) = 0
		_fondu("_fondu",Float) = 0
	_bluractivation("_bluractivation",Range(0,1)) = 0
	_powermodification("_powermodification",Range(0,1)) = 0
	_step2invert("_step2invert",Range(0,1)) = 0
	_dither("_dither",Range(0,1)) = 0
	[Toggle] _final("_final",Float) = 0
		_c1("_c1", Float) = 0
		_c2("_c2", Float) = 0
		_c3("_c3", Float) = 0
		_c4("_c4", Float) = 0
		/*_c5("_c5", Float) = 0
		_c6("_c6", Float) = 0
		_c7("_c7", Float) = 0*/
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            /*sampler2D _MainTex;
			sampler2D _MainTex2;
			sampler2D _MainTex3;*/
			sampler2D _MainTex4;
			sampler2D _bl;
			sampler2D _gui1;
			sampler2D _Date;
            float4 _MainTex4_ST;
			float _float1;
			float _float2;
			float _float3;
			float _float4;
			float _blur;
			float _step0to1;
			float _step1to2;
			float _bluractivation;
			float _powermodification;
			float _step2invert;
			float _dither;
			float _c1;
			float _c2;
			float _c3;
			float _c4;
			/*float _c5;
			float _c6;
			float _c7;*/
			float _final;
			float _fondu;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex4);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
			float3 exclusion(float3 s, float3 d)
			{
				return s + d - 2.0 * s * d;
			}
			float ov(float a, float b) {
				return a > 0.5 ? 2.*a*b : 1. - 2.*(1. - a)*(1. - b);
			}
			float rd(float t) { return frac(sin(dot(floor(t*12.), 45.))*7845.)+0.01; }
			float rs(float t) { return frac(sin(dot(t, 45.269))*7845.236); }
			float hs(float2 uv ) { float2 u = uv * float2(1920., 1080.) / 1024.; return sin(tex2D(_bl, u).x*6.2831853071 +_Time.y*30.)*0.5+0.5; }
			float hn(float2 uv) { float2 u = uv * float2(1920., 1080.) / 1024.; return (tex2D(_bl, u).x); }
			float no(float t) { return lerp(rd(t), rd(t + 1.), smoothstep(0., 1., frac(t))); }
			float rd2(float2 t, float ti) { return frac(sin(dot(floor(t), float2(45., 98.)))*(7845. + ti*12.)); }

			#define Pi 3.14159265359

            fixed4 frag (v2f i) : SV_target
            {
				float dt = 2. / 3.;
				float rf1 = step(0.5, rd(954.+_c3));
				float rf2 = step(dt, rd(98.+ _c3));
				float rf3 = step(0.15, rd(91.+ _c3));
				float r6 = step(0.5, rd(672.+ _c3));
				float r7 = step(0.5, rd(983. + _c3));
				float2 uv = (i.uv);
				float u1 = step(0.5, uv.x);
				float u2 = step(0.5, uv.y);
				float uto = lerp(u1, 1. - u1, r7);
				float utn = lerp(u2, 1. - u2, r7);
				float2 coy = lerp(uv, float2(uv.x, frac(uv.y + 0.5)), r6);
				float2 cox = lerp(uv, float2(frac(uv.x + 0.5), uv.y), r6);
				float2 uv2 = lerp(uv,lerp(lerp (cox,coy, r7), lerp(lerp(cox, uv, utn), lerp(coy, uv, uto), rf1), rf2),rf3*_step0to1);
				float Directions = 16.0;
				float Quality = 4.0;
				float Size =( 1.+hs(uv+986.5)*0.5)*_blur;
				float2 Radius = Size / float2(1920., 1080.);
				float3 c = float3(0., 0., 0.);
				for (float d = 0.0; d < Pi; d += Pi / Directions){
					for (float e = 1.0 / Quality; e <= 1.0; e += 1.0 / Quality){c += tex2D(_MainTex4, uv2 + float2(cos(d), sin(d))*Radius*e).xyz;}}
				c /= Quality * Directions - 15.;
				float3 b = tex2D(_MainTex4, uv2).xyz;
				float n1 = smoothstep(0.45, 0.55, no(_c2+98.))*_bluractivation;
				float n2 = smoothstep(0.45, 0.55, no(_c2 + 125.))*_bluractivation;
				float n3 = smoothstep(0.45, 0.55, no(_c2 + 78.))*_bluractivation;
				float ca = clamp(ov(lerp(b.x, c.x,  n1), lerp(0.5, hs(uv + 23.69), 0.2)), 0, 1.);
				float cb = clamp(ov(lerp(b.y, c.y,  n2),lerp(0.5, hs(uv + 23.69), 0.2)), 0., 1.);
				float cc = clamp(ov(lerp(b.z, c.z, n3), lerp(0.5, hs(uv + 23.69), 0.2)), 0., 1.);

				//////////////
				float t = _c1;
				float3 t1 = float3(ca,cb,cc);
				float3 t2 = float3(cb,cc,ca);
				float3 t3 = float3(cc,ca,cb);
				float l1 = step(0.5 / 1920., length(uv.x - 0.5));
				float l2 = step(0.5 / 1080., length(uv.y - 0.5));
				float rr1 = rd(65.+t);
				float r1 = step(0.5, rr1);
				float r2 = step(0.5, rd( 45.+t));
				float r3 = step(dt, rd( 78.+t));
				
				float bd = lerp(lerp(u1, 1. - u1, r2), 1., r3);
				float g1 = tex2D(_gui1, uv*float2(2., 1.)+float2(0.,0.077)).a*(1. - _final)*step(0.5, _powermodification)*bd;
				float3 tl1 = lerp(lerp(t1, t3, r2), t2, r3);
				float um1 = lerp(u1, u2, r7);
				float3 tm1 = lerp(t1, lerp(t3, t2, r1), um1);
				float3 tm2 = lerp(t2, lerp(t1, t3, r1), um1);
				float3 tm3 = lerp(t3, lerp(t1, t2, r1), um1);
				float3 tm4 =  lerp(lerp(tm1, tm2, r2), tm3, r3);

				float3 tn1 =  lerp(lerp(lerp(t2, t3, u1), lerp(t3, t2, u1), r1), t1, utn);
				float3 tn2 = lerp(lerp(lerp(t1, t3, u1), lerp(t3, t1, u1), r1), t2, utn);
				float3 tn3 = lerp(lerp(lerp(t2, t1, u1), lerp(t2, t1, u1), r1), t3, utn);
				float3 tn4 = lerp(lerp(tn1, tn2, r2), tn3, r3);
				
				float3 to1 =  lerp( lerp(lerp(t2, t3, u2), lerp(t3, t2, u2), r1), t1, uto);
				float3 to2 =  lerp( lerp(lerp(t1, t3, u2), lerp(t3, t1, u2), r1), t2, uto);
				float3 to3 =  lerp( lerp(lerp(t2, t1, u2), lerp(t2, t1, u2), r1), t3, uto);
				float3 to4 = lerp(lerp(to1, to2, r2), to3, r3);
				
				float3 tf1 = lerp(tl1,lerp(tm4, lerp(tn4, to4,rf1), rf2), rf3);

				float tll = 1.-lerp(1., lerp(lerp(l1,l2,r7), lerp(max(l1,utn)*l2, l1 *max(l2, uto), rf1), rf2), rf3);
				float g2 = lerp(g1, lerp(g1, lerp(g1, g1*u1, rf1), rf2), rf3);
				//////////
				float2 up = float2(frac(i.uv*4.)*float2(1., 4.)) - float2(0., 3.);
				up = (((up - 0.5)*2.)*1.1)*0.5 + 0.5;
				float tee = tex2D(_Date, up).x*(1.-u2);
				float2 up2 = float2(frac(i.uv*2.)*float2(1., 4.)) - float2(0., 3.3);
				up2 = (((up2 - 0.5)*2.)*2.2)*0.5 + 0.5;
				float tee2 = tex2D(_Date, up2).x*u2;
				float2 up3 = float2(frac(i.uv*2.)*float2(1., 4.)) - float2(-0.27, 3.3);
				up3 = (((up3 - 0.5)*2.)*2.2)*0.5 + 0.5;
				float tee4 = tex2D(_Date, up3).x*u2*(1.-u1);
				float tee3 = lerp(tee4,(tee2 + tee),_final);

				float3 pc = lerp(tl1,tf1, _step0to1);

				float pc1 = exclusion(pc.x, pc.y);
				float pc2 = exclusion(pc.x, pc.z);
				float pc3 = exclusion(pc.y, pc.z);
				float pc4 = lerp(lerp(pc1, pc2, smoothstep(0.45, 0.55, rd2(95.,_c1))), pc3, smoothstep(dt - 0.05*dt, dt + 0.05*dt, rd2( 451.,_c1)));
				float pc5 = clamp(pc4,0.,1.);
				float n4 = smoothstep(0.4, 0.95, no(_c4*0.00007+152.))*_step2invert;
				float pc6 = lerp(smoothstep(0.8, 0.1, pow(pc5, .5)), smoothstep(0.1, 0.9, pow(pc5, 1.)),1.-n4);

				float c2 = lerp(pc.x, pc6, _step1to2) ;
				float c3 = pow(c2,lerp(1.,lerp(0.75,1.5,no(_c4*0.0001)*0.+0.5),_powermodification));
				float c4 = step(hn(uv + 98.), (c3)*_float4)*no(_c4*0.0001 + 95.24)*_dither;

				float3 b1 = lerp(c3+c4, float3(0., 0., 1.), lerp(step(0.75,rd2(uv*4.,_c3*6.25))*(1.-u2),step(0.75,rd2(uv*2.,_c3*6.25))*u2,step(0.75,rr1))*_final);
				float lf = max((1. - step(0.5 / 1920.,min(length(uv.x - 0.5) , (u2)+length(abs(uv.x - 0.5) - 0.25)))), 1. - step(0.5 / 1080., min(length(uv.y - 0.5),length(uv.y - 0.25))));
				float3 b2 = lerp(b1, float3(1., 1., 1.),max(_final* lf,tll* _step0to1)+g2);
				
                return float4(lerp(lerp(smoothstep(1. - _fondu, 1., b2), b2, _fondu),float3(1.,1.,1.),tee3)*_fondu,1.);
            }
            ENDCG
        }
    }
}
/*

				float ti1 = _c1;
				float ti2 = _c2;
				float ti3 = _c3;
				float ti4 = _c4;
				float ti5 = _c2;
				float ti6 = _c3;

				float v6 = lerp(10., 1., smoothstep(0.1, 0.2, rd(ti6*0.5 + 785.)));
				float tv1 = rd(ti3*0.5 + 49.85);
				float v1 = pow(tv1, v6)*8.;
				float tv2 = rd(ti2*0.5 + 23.86);
				//float v2 = pow(lerp(tv2,no(ti2 + 23.),pow(min(tv1,tv2),2.)), v6)*40.;
				float v2 = pow(tv2, v6)*8.*v6;
				float tv3 = rd(ti3*0.5 + 78.58);
				float tv4 = rd(ti2*0.5 + 14.78);
				float v3 = pow(tv3, v6)*8.*v6;
				//float v3 = pow(lerp(tv3,no(ti3 + 78.), pow(min(tv3, tv4), 2.)), v6)*80.;
				float v4 = pow(tv4, v6)*8.;
				float tv5 = step(0.5, rd(ti4 + 452.));
				float2 v5 = lerp(float2(v1, v2), float2(v3, v4), tv5)*pow(rd(_c3*0.25 + 95.14), 3.);
				float tr1 = step(0.5, lerp(rd(uv.x*v1), rd(uv.y*v4), tv5));
				float r1 = step(no(ti5 + 95.), rd(uv*v5, ti1*0.25));
				float rf = lerp(1., r1, _step2invert);
*/