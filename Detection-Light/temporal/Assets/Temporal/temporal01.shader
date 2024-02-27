Shader "Unlit/webcam"
{
    Properties
    {

        _MainTex ("Texture", 2D) = "white" {}
		_handRight("_handRight",Vector)=(0.,0.,0.,0.)
			_handLeft("_handLeft",Vector) = (0.,0.,0.,0.)
			_head("_head",Vector) = (0.,0.,0.,0.)
			_shoulderRight("_shoulderRight", Vector) = (0., 0., 0., 0.)
			_shoulderLeft("_shoulderLeft", Vector) = (0., 0., 0., 0.)
			_elbowRight("_elbowRight", Vector) = (0., 0., 0., 0.)
			_elbowLeft("_elbowLeft", Vector) = (0., 0., 0., 0.)
			_hipRight("_hipRight", Vector) = (0., 0., 0., 0.)
			_hipLeft("_hipLeft", Vector) = (0., 0., 0., 0.)
			_kneeRight("_kneeRight", Vector) = (0., 0., 0., 0.)
			_kneeLeft("_kneeLeft", Vector) = (0., 0., 0., 0.)
			_ankleRight("_ankleRight", Vector) = (0., 0., 0., 0.)
			_ankleLeft("_ankleLeft", Vector) = (0., 0., 0., 0.)
			
			

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
			float4 _handRight;
			float4 _handLeft;
			float4 _head;
			float4 _hipRight;
			float4 _hipLeft;
			float4 _shoulderRight;
			float4 _shoulderLeft;
			float4 _elbowRight;
			float4 _elbowLeft;
			float4 _kneeRight;
			float4 _kneeLeft;
			float4 _ankleRight;
			float4 _ankleLeft;
			
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
			float li(float2 uv, float2  a, float2 b) {
				float2 ua = uv - a; float2 ba = b - a;
				float h = clamp(dot(ua, ba) / dot(ba, ba), 0., 1.);
				return length(ua - ba * h);
			}
			float smin(float d1, float d2, float k) {
				float h = clamp(0.5 + 0.5*(d2 - d1) / k, 0.0, 1.0);
				return lerp(d2, d1, h) - k * h*(1.0 - h);
			}
			
            fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);

			float k = 0.02;
			float t1 = 0.002;
			float va = 0.5;
			float l1 = max(li(i.uv, _handRight.xy, _elbowRight.xy), 1. - max(step(va, _handRight.z),step(va, _elbowRight.z)));
			float l2 = max(li(i.uv, _handLeft.xy, _elbowLeft.xy), 1. - max(step(va, _handLeft.z),step(va, _elbowLeft.z)));
			float l3 = max(li(i.uv, _shoulderRight.xy, _elbowRight.xy), 1. - max(step(va, _shoulderRight.z),step(va, _elbowRight.z)));
			float l4 = max(li(i.uv, _shoulderLeft.xy, _elbowLeft.xy), 1. - max(step(va, _shoulderLeft.z),step(va, _elbowLeft.z)));
			float l5 = max(li(i.uv, _hipRight.xy, _kneeRight.xy), 1. - max(step(va, _hipRight.z),step(va, _kneeRight.z)));
			float l6 = max(li(i.uv, _hipLeft.xy, _kneeLeft.xy), 1. - max(step(va, _hipLeft.z),step(va, _kneeLeft.z)));
			float l7 = max(li(i.uv, _ankleRight.xy, _kneeRight.xy), 1. - max(step(va, _ankleRight.z),step(va, _kneeRight.z)));
			float l8 = max(li(i.uv, _ankleLeft.xy, _kneeLeft.xy), 1. - max(step(va, _ankleLeft.z),step(va, _kneeLeft.z)));
			float l9 = max(li(i.uv, _hipLeft.xy, _hipRight.xy), 1. - max(step(va, _hipLeft.z),step(va, _hipRight.z)));
			float l10 = max(li(i.uv, _shoulderLeft.xy, _shoulderRight.xy), 1. - max(step(va, _shoulderLeft.z),step(va, _shoulderRight.z)));
			float l11 = max(li(i.uv, _head.xy, (_hipRight.xy + _hipLeft.xy)*0.5), 1. - step(va, _head.z));
			float lf = smin(smin(smin(smin(smin(smin(smin(smin(smin(smin(l1, l2,k), l3, k), l4, k), l5, k), l6, k), l7, k), l8, k), l9, k), l10, k), l11, k);
			float lf1 = smoothstep(0.003, 0.002, lf);

			return col+lf1;
			}
            ENDCG
        }
    }
}
