Shader "Unlit/shader01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	_a0("_a0",Float) = 0
		_b0("_b0",Float) = 0
		_c0("_c0",Float) = 0
		_d0("_d0",Float) = 0
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _a0;
			float _b0;
			float _c0;
			float _d0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
			float map(float value, float low1, float high1, float low2, float high2) {
				return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
			}
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               
			float d1 =_a0+_c0*.75;
			float d2 = _a0-_c0*0.75;
			float d3 = _b0-_d0*0.75;
			float d4 =  _b0+_d0*0.75;
			float ra = (_c0 / _d0);
			float ra2 = (_d0 / _c0);
			/*float ta; float ta2;
			if (ra < ra2) { ta = ra; ta2 = ra2; }
			else { ta = ra2, ta2 = ra; }*/
			float2 uv = clamp(((i.uv - 0.5)*2.*float2(ra2,1.))*0.5+0.5,0.,1.);
			float2 ut = float2(map(uv.x, 0., 1., d1, d2), map(uv.y,0., 1., d3, d4));
			
				fixed4 col = tex2D(_MainTex, lerp(ut,float2(1.-i.uv.x,i.uv.y),step(0.5,frac(_Time.y))));
                return col;
            }
            ENDCG
        }
    }
}
