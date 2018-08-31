using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour {
	[Header("Building")]
	public Shader buildingShader;
	public Shader previewShader;

	public int buildingShaderRenderQueue = 2000;
	public int previewShaderRenderQueue = 3000;

	public string colorMaterialPropertyName;
	public string effectMaterialPropertyName;
	public string fadeMaterialPropertyName;


	public void useBuildingShader() {
		foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
			foreach(Material material in renderer.materials) {
				material.shader = buildingShader;

				// Changing the render queue doesn't actually work, as the shader reassignment will cause it to
				// be overwritten later this frame. The render queue is hard-coded in the preview shader for now.
				material.renderQueue = buildingShaderRenderQueue;
			}

			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		}
	}

	public void usePreviewShader() {
		foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
			foreach(Material material in renderer.materials) {
				material.shader = previewShader;

				// Changing the render queue doesn't actually work, as the shader reassignment will cause it to
				// be overwritten later this frame. The render queue is hard-coded in the building shader for now.
				material.renderQueue = previewShaderRenderQueue;
			}

			// Otherwise all you see is shadow, lol.
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
	}
	

	public void setColor(Color color) {
		foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
			foreach(Material material in renderer.materials) {
				material.SetColor(colorMaterialPropertyName, color);
			}
		}
	}
	
	public void setEffect(float effect) {
		foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
			foreach(Material material in renderer.materials) {
				material.SetFloat(effectMaterialPropertyName, effect);
			}
		}
	}

	public void setFade(float fade) {
		foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
			foreach(Material material in renderer.materials) {
				material.shader = previewShader;
				material.SetFloat(fadeMaterialPropertyName, fade);
			}
		}
	}


	public void enableParticleSystems() {
		foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()) {
			if(!particleSystem.isPlaying) {
				particleSystem.Play();
			}
		}
	}
	
	public void disableParticleSystems() {
		foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()) {
			if(particleSystem.isPlaying) {
				particleSystem.Stop();
			}
		}
	}


	public void enableRadiators() {
		foreach(Radiator radiator in GetComponentsInChildren<Radiator>(true)) {
			radiator.gameObject.SetActive(true);
		}
	}
	
	public void disableRadiators() {
		foreach(Radiator radiator in GetComponentsInChildren<Radiator>(true)) {
			radiator.gameObject.SetActive(false);
		}
	}


	public virtual void evacuate() {
		disableParticleSystems();
		disableRadiators();
	}


	public float getHeight(Island island) {
		return transform.position.y - island.transform.position.y;
	}
}
