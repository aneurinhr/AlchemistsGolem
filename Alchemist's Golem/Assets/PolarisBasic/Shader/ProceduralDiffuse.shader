// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Pinwheel/ProceduralDiffuse" {
	Properties{
		_Color("Color", Color) = (0.75, 0.75, 0.75 ,1)
		_ColorByHeightPalette ("Color By Height Palette", 2D) = "white" {}
		_ColorByNormalPalette ("Color By Normal Palette", 2D) = "white" {}
		_ColorBlendPalette ("Color Blend Palette", 2D) = "black" {}
		_MaxHeight ("Max Height", Float) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		fixed4 _Color;
		sampler2D _ColorByHeightPalette;
		sampler2D _ColorByNormalPalette;
		sampler2D _ColorBlendPalette;
		fixed _MaxHeight;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed3 localPos = mul(unity_WorldToObject, IN.worldPos);
			fixed heightFraction = saturate(localPos.y/_MaxHeight);
			fixed normalFraction = saturate(dot(normalize(IN.worldNormal), fixed3(0,1,0)));
			
			fixed3 heightColor = tex2D(_ColorByHeightPalette, fixed2(heightFraction, 0.5)).rgb;
			fixed3 normalColor = tex2D(_ColorByNormalPalette, fixed2(normalFraction, 0.5)).rgb;
			fixed blendFraction = tex2D(_ColorBlendPalette, fixed2(heightFraction,0.5)).a;

			fixed4 blendedColor = fixed4(heightColor*(1-blendFraction)+normalColor*blendFraction,1);

			fixed4 c = _Color*blendedColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
