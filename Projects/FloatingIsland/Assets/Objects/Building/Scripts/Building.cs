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
	public string decayMaterialPropertyName;

	public float decayDuration;
	
	private float decayStartTime;
	
	
	protected virtual void Start() {
		// By initializing the start time really high, the decay will remain zero until we evacuate.
		decayStartTime = float.MaxValue;
	}

	protected virtual void Update() {
		float decayTime = Time.time - decayStartTime;
		float decay = decayTime / decayDuration;

		if(decay >= 0.0f && decay <= 1.0f) {
			setDecay(decay);
		}
	}


	public void useBuildingShader() {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.shader = buildingShader;

				// Changing the render queue doesn't actually work, as the shader reassignment will cause it to
				// be overwritten later this frame. The render queue is hard-coded in the building shader for now.
				material.renderQueue = buildingShaderRenderQueue;
			}

			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		}
	}

	public void usePreviewShader() {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.shader = previewShader;

				// Changing the render queue doesn't actually work, as the shader reassignment will cause it to
				// be overwritten later this frame. The render queue is hard-coded in the preview shader for now.
				material.renderQueue = previewShaderRenderQueue;
			}

			// Otherwise all you see is shadow, lol.
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
	}
	

	public void setColor(Color color) {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.SetColor(colorMaterialPropertyName, color);
			}
		}
	}
	
	public void setEffect(float effect) {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.SetFloat(effectMaterialPropertyName, effect);
			}
		}
	}

	public void setFade(float fade) {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.SetFloat(fadeMaterialPropertyName, fade);
			}
		}
	}

	public void setDecay(float decay) {
		foreach(Block block in GetComponentsInChildren<Block>()) {
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();

			foreach(Material material in renderer.materials) {
				material.SetFloat(decayMaterialPropertyName, decay);
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


	public void enableInformation() {
		foreach(Information information in GetComponentsInChildren<Information>(true)) {
			information.gameObject.SetActive(true);
		}
	}
	
	public void disableInformation() {
		foreach(Information information in GetComponentsInChildren<Information>(true)) {
			information.gameObject.SetActive(false);
		}
	}
	

	public void setInformation(string information) {
		foreach(Information component in GetComponentsInChildren<Information>(true)) {
			component.setInformation(information);
		}
	}


	public void setPrice(float price) {
		string value = "$" + price.ToString("n0");

		setInformation(value);
	}

	public void setRate(float rate) {
		// Round up to avoid float imprecisions preventing a 100% value.
		float percentage = Mathf.Ceil(100.0f * rate);

		string value = percentage.ToString("n0") + "%";

		setInformation(value);
	}


	public virtual void evacuate() {
		disableParticleSystems();
		disableRadiators();
		disableInformation();

		// Start the decay.
		decayStartTime = Time.time;
	}


	public float getHeight(Island island) {
		return transform.position.y - island.transform.position.y;
	}
	
	public float getLevel(Island island) {
		return getHeight(island) + island.getLevel();
	}


	public bool isFlooded(Island island, Water water) {
		return getLevel(island) < water.getLevel();
	}


	public virtual void run(Manager manager) {
		// Default implementation is to do nothing.
	}
}
