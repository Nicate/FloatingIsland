using UnityEngine;

public class Silo : Building {
	[Header("Silo")]
	public float capacity = 0.0f;


	private float storage;

	
	private void Start() {
		storage = 0.0f;
	}


	public float getStorage() {
		return storage;
	}

	private void setStorage(float storage) {
		this.storage = Mathf.Clamp(storage, 0, capacity);
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
