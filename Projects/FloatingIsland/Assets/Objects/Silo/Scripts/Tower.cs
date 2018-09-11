using UnityEngine;

public class Tower : Block {
	public float minimum = 0.0f;
	public float maximum = 1.0f;

	public float effectMultiplier = 1.0f;

	public Color color;

	public int materialIndex;
	
	public string colorMaterialPropertyName;
	public string effectMaterialPropertyName;


	private float fill;


	private void Start() {
		setFill(0.0f);
	}


	public float getFill() {
		return fill;
	}

	public void setFill(float fill) {
		this.fill = fill;

		updateMaterial();
	}


	private void updateMaterial() {
		Material material = GetComponent<MeshRenderer>().materials[materialIndex];

		float effect = effectMultiplier * Mathf.Clamp01((fill - minimum) / (maximum - minimum));
		
		material.SetColor(colorMaterialPropertyName, color);
		material.SetFloat(effectMaterialPropertyName, effect);
	}
}
