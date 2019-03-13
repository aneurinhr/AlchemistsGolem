// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Pinwheel/Intersection Highlight" 
{
	Properties{
		_Color ("Color", Color) = (1,1,1,1)
		_IntersectColor("Intersect color", Color) = (1,1,1,1)
		_Threshold("Threshold", Float) = 2
		_AnimationSpeed ("Animation Speed", Float) = 10
	}
   SubShader {
      Tags { "Queue" = "Transparent" } 
      Pass {
	  Cull Off
         ZWrite Off 
         Blend SrcAlpha OneMinusSrcAlpha

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
		#include "UnityCG.cginc"

		struct VertexInput{
			float4 vertex: POSITION;
			float2 uv: TEXCOORD0;
			float3 normal: NORMAL;
		};

		struct VertexOutput{
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
			float4 screenPos: TEXCOORD1;
			float3 normal:NORMAL;
		};

		float4 _Color;
		float4 _IntersectColor;
		float _Threshold;
		float _AnimationSpeed;
		sampler2D _CameraDepthTexture;

         VertexOutput vert(VertexInput i)
         {
			VertexOutput o;
			o.pos = UnityObjectToClipPos(i.vertex);
			o.screenPos = ComputeScreenPos(o.pos);
			o.uv = i.uv;
			o.normal=i.normal;
            return o;
         }
 
         float4 frag(VertexOutput i) : COLOR 
         {
            half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
			half intersectStrength = step(depth-i.screenPos.w, _Threshold);
			half4 intersectColor = _IntersectColor*intersectStrength;
			half animMultiplier = (cos(_Time.y*_AnimationSpeed)+8)/9; //clamp to 0.9->1

			half normalMultiplier = 1-abs(dot(i.normal, float3(0,1,0)));
			half uvMultiplier = (1-i.uv.y)*(1-i.uv.y);
			return (1-intersectStrength)*float4(_Color.rgb, _Color.a*uvMultiplier*normalMultiplier) + intersectColor*animMultiplier;
         }
 
         ENDCG  
      }
   }
}