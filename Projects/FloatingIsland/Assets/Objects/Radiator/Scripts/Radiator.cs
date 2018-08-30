using UnityEngine;

public class Radiator : MonoBehaviour {
	private void Start() {
		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}
}
