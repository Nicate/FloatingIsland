Shader "Island/Building" {
	Properties {
		[Header(Building)]
		[Toggle] Building_Metal("Metal", Float) = 0.0
		[Space]
		[NoScaleOffset] Building_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Building_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Building_RoughnessTexture("Roughness", 2D) = "white" {}
		[NoScaleOffset] Building_HeightTexture("Height", 2D) = "black" {}
		[PowerSlider(2.0)] Building_TextureScale("Scale", Range(0.0625, 16.0)) = 1.0

		[Header(Grunge)]
		[Toggle] Grunge_Metal("Metal", Float) = 0.0
		[Space]
		[NoScaleOffset] Grunge_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Grunge_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Grunge_RoughnessTexture("Roughness", 2D) = "white" {}
		[NoScaleOffset] Grunge_HeightTexture("Height", 2D) = "black" {}
		[PowerSlider(2.0)] Grunge_TextureScale("Scale", Range(0.0625, 16.0)) = 1.0

		[Header(Transition)]
		Grunge("Grunge", Range(0.0, 1.0)) = 0.0
		Depth("Depth", Range(0.01, 1.0)) = 0.2

		[Header(Color)]
		AlbedoColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		Effect("Effect", Range(0.0, 1.0)) = 0.0

		[Header(Fade)]
		Fade("Fade", Range(0.0, 1.0)) = 0.0

		[Header(Decay)]
		Decay("Decay", Range(0.0, 1.0)) = 0.0
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
