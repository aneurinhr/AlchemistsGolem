// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Pinwheel/Water" 
{
	Properties{
		_Color ("Color", Color) = (1,1,1,1)
		_FoamColor("Foam color", Color) = (1,1,1,1)
		_FoamStrengthMin ("Foam Strength Min", Float) = 1
		_FoamStrengthMax("Foam Strength Max", Float) = 2
		_AnimationSpeed ("Animation Speed", Float) = 10
		_DepthTint ("Depth Tint", Range(-1.0,1.0))= 1
		_MaxDepth ("Max Depth", Float) = 10
		_Fresnel ("Fresnel", Range(0.0,1.0)) = 0.2
		_WaveHeight ("Wave Height", Float) = 1
		_WaveFrequency ("Wave Frequency", Float) = 1
		_DistortionStrength ("Distortion Strength", Float) = 1
	}
   SubShader {
      Tags { "Queue" = "Transparent" } 

	  GrabPass{
	  	  "_UnderwaterTexture"
	  }

      Pass {
	  Cull Back
         ZWrite Off 
		 Blend Off
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
		 #pragma target 3.0
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		struct VertexInput{
			float4 vertex: POSITION;
			float2 uv: TEXCOORD0;
			float3 normal: NORMAL;
		};

		struct VertexOutput{
			float4 pos: SV_POSITION;
			float2 uv: TEXCOORD0;
			float4 screenPos: TEXCOORD1;
			float4 worldPos: TEXCOORD2;
			float3 worldNormal:NORMAL;
			float3 directionalLightColor:COLOR;
			float4 grabPos: TEXCOORD3;
		};

		half4 _Color;
		half4 _FoamColor;
		half _FoamStrengthMin;
		half _FoamStrengthMax;
		half _AnimationSpeed;
		half _DepthTint;
		half _MaxDepth;
		half _Fresnel;
		half _WaveHeight;
		half _WaveFrequency;
		half _DistortionStrength;
		sampler2D _CameraDepthTexture;
		sampler2D _UnderwaterTexture;

         VertexOutput vert(VertexInput i)
         {
			float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
			worldPos.y += sin((worldPos.x*worldPos.z)*_WaveFrequency*0.01+_AnimationSpeed*_Time.x)*_WaveHeight;
			
			i.vertex = mul(unity_WorldToObject, worldPos);

			VertexOutput o;
			o.pos = UnityObjectToClipPos(i.vertex);
			o.screenPos = ComputeScreenPos(o.pos);
			o.uv = i.uv;
			o.worldPos = worldPos;

			half3 worldNormal = UnityObjectToWorldNormal(i.normal);
			half lightStrength = abs(dot(worldNormal, _WorldSpaceLightPos0));
			o.directionalLightColor = _Color*_LightColor0*lightStrength;
			o.worldNormal=worldNormal;

			o.grabPos = ComputeGrabScreenPos(o.pos);

            return o;
         }
 
         half4 frag(VertexOutput i) : COLOR 
         {
            half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
			
			half threshold = lerp(_FoamStrengthMin, _FoamStrengthMax,(cos((i.worldPos.x+i.worldPos.z)*0.5 + _Time.x*_AnimationSpeed)+1)/2);
			half foamStrength = step(depth-i.screenPos.w, threshold);
			half4 foamColor = _FoamColor*foamStrength;

			half depthFraction =saturate(abs(depth-i.screenPos.w)/_MaxDepth);
			half3 depthColor = _Color.rgb*depthFraction*_DepthTint;

			half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
			half fresnelStrength =pow(1- dot(viewDir, i.worldNormal), 1/_Fresnel);
			half3 fresnelColor = _Color.rgb*fresnelStrength;

			i.grabPos += sin((i.worldPos.x+i.worldPos.z) + _Time.x*_AnimationSpeed)*_DistortionStrength;
			float4 bgColor = tex2Dproj(_UnderwaterTexture, i.grabPos);

			half3 baseColor = (i.directionalLightColor.rgb + depthColor + foamColor + fresnelColor);
			half alpha = saturate(_Color.a + depthFraction);
			half4 result = float4(baseColor.rgb, alpha)*alpha + bgColor*(1-alpha);
			
			return result;
         }
 
         ENDCG  
      }
   }
}