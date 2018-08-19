Shader "Island/Radiator" {
	Properties {
		[Header(Radiator)]
		[NoScaleOffset] AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] RoughnessTexture("Roughness", 2D) = "black" {}
		[PowerSlider(2.0)] TextureScale("Scale", Range(0.125, 8.0)) = 1.0
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
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
			#pragma surface surf Standard alpha:fade vertex:vert

			#pragma target 3.0


			float Animation;

			float RotationSpeed;
			float TranslationSpeed;

			float Radius;


			struct Input {
				float3 position;
				float2 coordinates;

				// Provided by Unity.
				float4 screenPos;
			};


			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				float angle = _Time.y * UNITY_TWO_PI * RotationSpeed * Animation;

				// Wrap the scale into the rotation while we're at it.
				float cosine = cos(angle) * Radius;
				float sine = sin(angle) * Radius;

				float3x3 rotation = float3x3(cosine, 0.0, sine, 0.0, 1.0, 0.0, -sine, 0.0, cosine);

				data.vertex.xyz = mul(rotation, data.vertex.xyz);

				float distance = _Time.y * TranslationSpeed * Animation;

				data.texcoord.x = data.texcoord.x * Radius;
				data.texcoord.y = data.texcoord.y + distance;

				// We provide normals in object space.
				data.tangent = float4(1.0, 0.0, 0.0, 1.0);
				data.normal = float4(0.0, 0.0, 1.0, 0.0);

				input.position = data.vertex.xyz;
				input.coordinates = data.texcoord.xy;
			}


			sampler2D AlbedoTexture;
			sampler2D NormalTexture;
			sampler2D RoughnessTexture;
			float TextureScale;

			float4 AlbedoColor;
		
			float Depth;
			float Exponent;

			float MinimumTransparency;
			float MaximumTransparency;


			sampler2D _CameraDepthTexture;


			struct Material {
				sampler2D albedos;
				sampler2D normals;
				sampler2D roughnesses;
			};

			Material createMaterial(sampler2D albedos, sampler2D normals, sampler2D roughnesses) {
				Material material;

				material.albedos = albedos;
				material.normals = normals;
				material.roughnesses = roughnesses;

				return material;
			}


			struct MaterialSample {
				float4 albedo;
				float3 normal;
				float roughness;
			};

			MaterialSample createSample(float4 albedo, float3 normal, float roughness) {
				MaterialSample materialSample;

				materialSample.albedo = albedo;
				materialSample.normal = normal;
				materialSample.roughness = roughness;

				return materialSample;
			}


			MaterialSample sampleMaterial(Material material, float2 coordinates) {
				float4 albedo = tex2D(material.albedos, coordinates);
				float3 normal = UnpackNormal(tex2D(material.normals, coordinates));
				float roughness = tex2D(material.roughnesses, coordinates).r;

				return createSample(albedo, normal, roughness);
			}


			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float2 coordinates = input.coordinates * TextureScale;
				float4 screenPosition = UNITY_PROJ_COORD(input.screenPos);

				Material material = createMaterial(AlbedoTexture, NormalTexture, RoughnessTexture);
				MaterialSample materialSample = sampleMaterial(material, coordinates);
				
				float3 bitangent = float3(0.0, 1.0, 0.0);
				float3 normal = normalize(position);
				float3 tangent = normalize(cross(normal, bitangent));

				float3x3 objectSpaceToTangentSpace = float3x3(tangent, bitangent, normal);
				float3x3 tangentSpaceToObjectSpace = transpose(objectSpaceToTangentSpace);

				materialSample.normal = mul(tangentSpaceToObjectSpace, materialSample.normal);

				// The abs() removes (anti-aliasing?) artifacts.
				float sceneDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, screenPosition).r);
				float fragmentDepth = LinearEyeDepth(screenPosition.z / screenPosition.w);
				float depth = abs(sceneDepth - fragmentDepth);

				float transparency = 1.0 - saturate(depth / Depth);
				transparency = lerp(MinimumTransparency, MaximumTransparency, transparency);
				transparency = pow(transparency, Exponent);

				output.Albedo = materialSample.albedo.rgb * AlbedoColor.rgb;
				output.Alpha = materialSample.albedo.a * AlbedoColor.a * transparency;
				output.Normal = materialSample.normal;
				output.Smoothness = 1.0 - materialSample.roughness;
				output.Metallic = 0.0;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
