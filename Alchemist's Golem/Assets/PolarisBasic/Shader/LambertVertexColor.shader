// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Pinwheel/LambertVertexColor" {
	Properties{
		_Color ("Color", Color) = (0.75, 0.75, 0.75 ,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		struct Input {
			float4 vertexColor: COLOR;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = IN.vertexColor * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
