using UnityEngine;

public class Water : MonoBehaviour {
	public float speed = 0.0f;

	
	private float level;
	private float targetLevel;


	private void Start() {
		level = transform.position.y;
		targetLevel = transform.position.y;

		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}

	
	private void Update() {
		// We can only go up.
		if(level < targetLevel) {
			level += speed * Time.deltaTime;

			if(level > targetLevel) {
				level = targetLevel;
			}
		}

		Vector3 position = transform.position;
		position.y = level;
		transform.position = position;
	}


	public float getLevel() {
		return targetLevel;
	}

	public void setLevel(float level) {
		targetLevel = level;
	}
}
