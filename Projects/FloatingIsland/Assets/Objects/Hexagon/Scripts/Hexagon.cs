using UnityEngine;

public class Hexagon : MonoBehaviour {
	public string baseMaterialPropertyName;


	public void enableBasePlate() {
		Material material = GetComponent<MeshRenderer>().material;

		material.SetFloat(baseMaterialPropertyName, 1.0f);
	}

	public void disableBasePlate() {
		Material material = GetComponent<MeshRenderer>().material;

		material.SetFloat(baseMaterialPropertyName, 0.0f);
	}
}
