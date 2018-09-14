using UnityEngine;

public class Information : MonoBehaviour {
	private string information;


	private void Start() {
		information = "";

		updateRotation();
	}
	

	private void LateUpdate() {
		updateRotation();
	}


	private void updateRotation() {
		transform.rotation = Camera.main.transform.rotation;
	}


	public string getInformation() {
		return information;
	}

	public void setInformation(string information) {
		this.information = information;

		updateText();
	}


	private void updateText() {
		GetComponentInChildren<TextMesh>(true).text = information;
	}
}
