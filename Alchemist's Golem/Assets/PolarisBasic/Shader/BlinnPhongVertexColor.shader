// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Pinwheel/BlinnPhongVertexColor" {
	Properties{
		_Color("Color", Color) = (0.75, 0.75, 0.75 ,1)
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Specular("Specular", Range(0.0,1.0)) = 0.5
		_Gloss ("Gloss", Float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		#pragma surface surf BlinnPhong fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		struct Input {
			float4 vertexColor: COLOR;
		};

		fixed4 _Color;
		fixed _Specular;
		fixed _Gloss;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = IN.vertexColor * _Color;
			o.Albedo = c.rgb;
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
