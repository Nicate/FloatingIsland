using UnityEngine;

public class Water : MonoBehaviour {
	public float speed = 0.0f;


	private float targetLevel;


	private void Start() {
		// Enable the depth texture so we can do depth shading.
		Camera.main.depthTextureMode = DepthTextureMode.Depth;

		targetLevel = transform.position.y;
	}
	
	private void Update() {
		float delta = speed * Time.deltaTime;
		
		Vector3 position = transform.position;
		
		float level = position.y;

		// We can only go up.
		if(level < targetLevel) {
			level += delta;

			if(level > targetLevel) {
				level = targetLevel;
			}
		}

		position.y = level;

		transform.position = position;
	}


	public void rise(float level) {
		targetLevel = level;
	}


	public float getLevel() {
		return transform.position.y;
	}
}
