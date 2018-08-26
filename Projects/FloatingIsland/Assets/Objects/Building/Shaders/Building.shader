Shader "Island/Building" {
	Properties {
		[Header(Building)]
		[NoScaleOffset] AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] RoughnessTexture("Roughness", 2D) = "white" {}
		[PowerSlider(2.0)] TextureScale("Scale", Range(0.0625, 16.0)) = 1.0

		[Header(Metal)]
		[Toggle] Metal("Metal", Float) = 0.0

		[Header(Color)]
		AlbedoColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		Effect("Effect", Range(0.0, 1.0)) = 0.0

		[Header(Fade)]
		Fade("Fade", Range(0.0, 1.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
			// This is the default (opaque) building shader.
			#pragma surface surf Standard

			#pragma target 3.0


			// Share functionality with the preview shader.
			#include "Building.cginc"
		ENDCG
	}
	FallBack "Diffuse"
}
