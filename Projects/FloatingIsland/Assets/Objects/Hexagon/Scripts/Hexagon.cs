using UnityEngine;

public class Hexagon : MonoBehaviour {
	public string baseMaterialPropertyName;


	private bool occupied;
	private Building occupant;


	public bool isOccupied() {
		return occupied;
	}

	public Building getOccupant() {
		if(occupied) {
			return occupant;
		}
		else {
			return null;
		}
	}

	public void occupy(Building building) {
		occupant = building;
		occupied = true;
	}


	public void enableBasePlate() {
		Material material = GetComponent<MeshRenderer>().material;

		material.SetFloat(baseMaterialPropertyName, 1.0f);
	}

	public void disableBasePlate() {
		Material material = GetComponent<MeshRenderer>().material;

		material.SetFloat(baseMaterialPropertyName, 0.0f);
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
}
