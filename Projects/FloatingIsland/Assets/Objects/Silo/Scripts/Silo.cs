using UnityEngine;

public class Silo : Building {
	[Header("Silo")]
	public float capacity = 0.0f;

	[Header("Towers")]
	public int towers = 0;


	private float storage;

	
	private void Start() {
		storage = 0.0f;
	}


	public override void run(Manager manager) {
		int index = 0;

		float rate = storage / capacity;

		foreach(Tower tower in GetComponentsInChildren<Tower>()) {
			float minimum = index / (float) towers;
			float maximum = (index + 1) / (float) towers;

			float fill = Mathf.Clamp01((rate - minimum) / (maximum - minimum));

			tower.setFill(fill);

			index += 1;

			if(index == towers) {
				break;
			}
		}
	}


	public float getStorage() {
		return storage;
	}

	private void setStorage(float storage) {
		this.storage = Mathf.Clamp(storage, 0.0f, capacity);
	}


	public bool hasRoom() {
		return storage < capacity;
	}
	
	public bool hasProduct() {
		return storage > 0.0f;
	}


	public float deposit(float produced) {
		float buffer = storage + produced;

		storage = Mathf.Clamp(buffer, 0, capacity);

		return produced - buffer + storage;
	}
	
	public float withdraw(float consumed) {
		float buffer = storage - consumed;

		storage = Mathf.Clamp(buffer, 0, capacity);

		return consumed - storage + buffer;
	}
}
