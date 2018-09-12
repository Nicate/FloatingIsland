using UnityEngine;

public class Viewer : MonoBehaviour {
	[Header("Focus")]
	public GameObject focus;

	[Header("Settings")]
	public float rotateSensitivity = 1.0f;
	public float zoomSensitivity = 1.0f;

	public float startPitch = 0.0f;
	public float startTilt = 0.0f;
	public float startZoom = 0.0f;

	public float minimumPitch = 0.0f;
	public float maximumPitch = 0.0f;
	
	public float minimumZoom = 0.0f;
	public float maximumZoom = 0.0f;
	
	[Header("Input")]
	public MouseButton pitchAndTiltMouseButton = MouseButton.Right;
	public string zoomAxis = "Mouse ScrollWheel";


	public enum MouseButton {
		Left,
		Right,
		Middle
	}

	
	private Camera viewer;

	private GameObject rig;
	private GameObject boom;

	private bool rotating;
	private Vector3 mousePosition;

	private float pitch;
	private float tilt;

	private float zoom;


	private void Awake() {
		rig = new GameObject("Rig");
		rig.transform.SetParent(transform.parent, false);

		boom = new GameObject("Boom");
		boom.transform.SetParent(rig.transform, false);
		
		viewer = GetComponent<Camera>();
		viewer.transform.SetParent(boom.transform, false);

		viewer.transform.localPosition = Vector3.zero;
		viewer.transform.localRotation = Quaternion.identity;
		viewer.transform.localScale = Vector3.one;
	}
	
	private void Start() {
		rotating = false;

		pitch = startPitch;
		tilt = startTilt;

		zoom = startZoom;

		updatePosition();
		updateRotation();
		updateZoom();
	}

	
	private void Update() {
		int mouseButton = (int) pitchAndTiltMouseButton;

		bool rightMouseDown = Input.GetMouseButtonDown(mouseButton);
		bool rightMouseUp = Input.GetMouseButtonUp(mouseButton);

		if(rotating) {
			float distance = Vector3.Distance(focus.transform.position, viewer.transform.position);
			
			Vector3 delta = (Input.mousePosition - mousePosition) * distance * rotateSensitivity;

			pitch -= delta.y;
			tilt += delta.x;

			pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
			tilt = tilt % 360.0f;

			mousePosition = Input.mousePosition;
		}

		if(rightMouseDown) {
			mousePosition = Input.mousePosition;

			rotating = true;
		}

		if(rightMouseUp) {
			mousePosition = Vector3.zero;

			rotating = false;
		}

		zoom -= Input.GetAxis(zoomAxis) * zoomSensitivity;
		zoom = Mathf.Clamp(zoom, minimumZoom, maximumZoom);

		// These are calculated independent of other components.
		updateRotation();
		updateZoom();
	}
	
	private void LateUpdate() {
		// This is calculated dependent on another component.
		updatePosition();
	}


	private void updatePosition() {
		rig.transform.position = focus.transform.position;
	}
	
	private void updateRotation() {
		boom.transform.localRotation = Quaternion.Euler(pitch, tilt, 0.0f);
	}

	private void updateZoom() {
		float distance = Mathf.Exp(zoom);

		viewer.transform.localPosition = new Vector3(0.0f, 0.0f, -distance);
	}
}