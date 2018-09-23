struct Input {
	float2 uvBuilding_AlbedoTexture;
	float2 uvGrunge_AlbedoTexture;
};


float Building_Metal;

sampler2D Building_AlbedoTexture;
sampler2D Building_NormalTexture;
sampler2D Building_RoughnessTexture;
sampler2D Building_HeightTexture;
float Building_TextureScale;

float Grunge_Metal;

sampler2D Grunge_AlbedoTexture;
sampler2D Grunge_NormalTexture;
sampler2D Grunge_RoughnessTexture;
sampler2D Grunge_HeightTexture;
float Grunge_TextureScale;

float Grunge;
float Depth;

float4 AlbedoColor;
float Effect;

float Fade;

float Decay;


struct Material {
	float metal;

	sampler2D albedos;
	sampler2D normals;
	sampler2D roughnesses;
	sampler2D heights;
};

Material createMaterial(float metal, sampler2D albedos, sampler2D normals, sampler2D roughnesses, sampler2D heights) {
	Material material;

	material.metal = metal;

	material.albedos = albedos;
	material.normals = normals;
	material.roughnesses = roughnesses;
	material.heights = heights;

	return material;
}


struct MaterialSample {
	float3 albedo;
	float3 normal;
	float roughness;
	float metallic;
	float height;
};

MaterialSample createSample(float3 albedo, float3 normal, float roughness, float metallic, float height) {
	MaterialSample materialSample;

	materialSample.albedo = albedo;
	materialSample.normal = normal;
	materialSample.roughness = roughness;
	materialSample.metallic = metallic;
	materialSample.height = height;

	return materialSample;
}


MaterialSample sampleMaterial(Material material, float2 coordinates) {
	float3 albedo = tex2D(material.albedos, coordinates).rgb;
	float3 normal = UnpackNormal(tex2D(material.normals, coordinates));
	float roughness = tex2D(material.roughnesses, coordinates).r;
	float metallic = material.metal;
	float height = tex2D(material.heights, coordinates).r;

	return createSample(albedo, normal, roughness, metallic, height);
}


/**
 * Based on http://www.gamasutra.com/blogs/AndreyMishkinis/20130716/196339/Advanced_Terrain_Texture_Splatting.php,
 * but truncates the depth to the blend value so height blending does not occur outside of the interpolation interval.
 */
MaterialSample blendSamples(MaterialSample materialSample1, MaterialSample materialSample2, float blend, float depth) {
	float2 heights = float2(materialSample1.height, materialSample2.height);
	float2 blends = float2(1.0 - blend, blend);

	heights = blends + heights;

	float height = max(heights.x, heights.y);
	float2 depths = min(depth, blends);
	float2 weights = saturate(heights - (height - depths));

	float3 albedo = (materialSample1.albedo * weights.x + materialSample2.albedo * weights.y) / (weights.x + weights.y);
	float3 normal = normalize(materialSample1.normal * weights.x + materialSample2.normal * weights.y);
	float roughness = (materialSample1.roughness * weights.x + materialSample2.roughness * weights.y) / (weights.x + weights.y);
	float metallic = lerp(materialSample1.metallic, materialSample2.metallic, step(weights.x, weights.y));
	float sampleHeight = (materialSample1.height * weights.x + materialSample2.height * weights.y) / (weights.x + weights.y);

	return createSample(albedo, normal, roughness, metallic, sampleHeight);
}

			
void surf(Input input, inout SurfaceOutputStandard output) {
	float2 buildingCoordinates = input.uvBuilding_AlbedoTexture * Building_TextureScale;

	Material buildingMaterial = createMaterial(Building_Metal, Building_AlbedoTexture, Building_NormalTexture, Building_RoughnessTexture, Building_HeightTexture);
	MaterialSample buildingMaterialSample = sampleMaterial(buildingMaterial, buildingCoordinates);

	float2 grungeCoordinates = input.uvGrunge_AlbedoTexture * Grunge_TextureScale;

	Material grungeMaterial = createMaterial(Grunge_Metal, Grunge_AlbedoTexture, Grunge_NormalTexture, Grunge_RoughnessTexture, Grunge_HeightTexture);
	MaterialSample grungeMaterialSample = sampleMaterial(grungeMaterial, grungeCoordinates);

	MaterialSample materialSample = blendSamples(buildingMaterialSample, grungeMaterialSample, Grunge * Decay, Depth);

	float3 albedo = lerp(materialSample.albedo, AlbedoColor.rgb, Effect);
	float alpha = lerp(AlbedoColor.a, 0.0, Fade);

	output.Albedo = albedo;
	output.Alpha = alpha;
	output.Normal = materialSample.normal;
	output.Smoothness = 1.0 - materialSample.roughness;
	output.Metallic = materialSample.metallic;
}