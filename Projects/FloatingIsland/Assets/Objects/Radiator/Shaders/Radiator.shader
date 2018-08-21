﻿Shader "Island/Radiator" {
	Properties {
		[Header(Radiator)]
		[NoScaleOffset] AlphaTexture("Alpha", 2D) = "white" {}
		[PowerSlider(2.0)] TextureScale("Scale", Range(0.125, 8.0)) = 1.0
		MinimumAlpha("Minimum Alpha", Range(0.0, 1.0)) = 0.0
		MaximumAlpha("Maximum Alpha", Range(0.0, 1.0)) = 1.0
		AlbedoColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)

		[Header(Depth)]
		[PowerSlider(2.0)] Depth("Depth", Range(0.1, 10.0)) = 1.0
		[PowerSlider(2.0)] Exponent("Exponent", Range(0.1, 10.0)) = 1.0
		MinimumTransparency("Minimum Transparency", Range(0.0, 1.0)) = 0.0
		MaximumTransparency("Maximum Transparency", Range(0.0, 1.0)) = 1.0

		[Header(Animation)]
		[Toggle] Animation("Animation", Float) = 0.0
		RotationSpeed("Rotation Speed", Range(-2.0, 2.0)) = 0.0
		TranslationSpeed("Translation Speed", Range(-2.0, 2.0)) = 0.0

		[Header(Size)]
		[PowerSlider(2.0)] Radius("Radius", Range(0.1, 10.0)) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "ForceNoShadowCasting"="True" }
		LOD 200

		CGPROGRAM
			// Use a custom lighting model since we are not physically based.
			#pragma surface surf Flat alpha:fade vertex:vert noshadow noforwardadd

			#pragma target 3.0


			float Animation;

			float RotationSpeed;
			float TranslationSpeed;

			float Radius;


			struct Input {
				float2 coordinates;

				// Provided by Unity.
				float4 screenPos;
			};


			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				// Make the edge translation speed independent of the radius.
				float angle = _Time.y * UNITY_TWO_PI * RotationSpeed / Radius * Animation;

				// Wrap the scale into the rotation while we're at it.
				float cosine = cos(angle) * Radius;
				float sine = sin(angle) * Radius;

				float3x3 rotation = float3x3(cosine, 0.0, sine, 0.0, 1.0, 0.0, -sine, 0.0, cosine);

				data.vertex.xyz = mul(rotation, data.vertex.xyz);
				data.normal.xyz = mul(rotation, data.normal.xyz);

				// Keep the resolution about the same while still wrapping seamlessly.
				float radius = ceil(Radius);

				float distance = _Time.y * TranslationSpeed * Animation;

				data.texcoord.x = data.texcoord.x * radius;
				data.texcoord.y = data.texcoord.y + distance;

				input.coordinates = data.texcoord.xy;
			}


			sampler2D AlphaTexture;
			float TextureScale;

			float MinimumAlpha;
			float MaximumAlpha;

			float4 AlbedoColor;

			float Depth;
			float Exponent;

			float MinimumTransparency;
			float MaximumTransparency;


			sampler2D _CameraDepthTexture;


			void surf(Input input, inout SurfaceOutput output) {
				float2 coordinates = input.coordinates * TextureScale;
				float4 screenPosition = UNITY_PROJ_COORD(input.screenPos);

				float alpha = tex2D(AlphaTexture, coordinates).r;
				alpha = lerp(MinimumAlpha, MaximumAlpha, alpha);

				// The abs() removes (anti-aliasing?) artifacts.
				float sceneDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, screenPosition).r);
				float fragmentDepth = LinearEyeDepth(screenPosition.z / screenPosition.w);
				float depth = abs(sceneDepth - fragmentDepth);

				float transparency = 1.0 - saturate(depth / Depth);
				transparency = lerp(MinimumTransparency, MaximumTransparency, transparency);
				transparency = pow(transparency, Exponent);

				output.Albedo = AlbedoColor.rgb;
				output.Alpha = alpha * AlbedoColor.a * transparency;

				// If we don't provide a normal the compiler flips out.
				output.Normal = float3(0.0, 1.0, 0.0);
			}
			
			
			float4 LightingFlat(SurfaceOutput output, float3 direction, float attenuation) {
				// Only deal with ambient lighting and fog and so on.
				return float4(output.Albedo, output.Alpha);
			}
		ENDCG
	}
	FallBack "Diffuse"
}