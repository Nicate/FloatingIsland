struct Input {
	float2 uvAlbedoTexture;
};


sampler2D AlbedoTexture;
sampler2D NormalTexture;
sampler2D RoughnessTexture;
float TextureScale;

float Metal;

float4 AlbedoColor;
float Effect;

float Fade;


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
	float3 albedo;
	float3 normal;
	float roughness;
};

MaterialSample createSample(float3 albedo, float3 normal, float roughness) {
	MaterialSample materialSample;

	materialSample.albedo = albedo;
	materialSample.normal = normal;
	materialSample.roughness = roughness;

	return materialSample;
}


MaterialSample sampleMaterial(Material material, float2 coordinates) {
	float3 albedo = tex2D(material.albedos, coordinates).rgb;
	float3 normal = UnpackNormal(tex2D(material.normals, coordinates));
	float roughness = tex2D(material.roughnesses, coordinates).r;

	return createSample(albedo, normal, roughness);
}

			
void surf(Input input, inout SurfaceOutputStandard output) {
	float2 coordinates = input.uvAlbedoTexture * TextureScale;

	Material material = createMaterial(AlbedoTexture, NormalTexture, RoughnessTexture);
	MaterialSample materialSample = sampleMaterial(material, coordinates);

	float3 albedo = lerp(materialSample.albedo, AlbedoColor.rgb, Effect);
	float alpha = lerp(AlbedoColor.a, 0.0, Fade);

	output.Albedo = albedo;
	output.Alpha = alpha;
	output.Normal = materialSample.normal;
	output.Smoothness = 1.0 - materialSample.roughness;
	output.Metallic = Metal;
}