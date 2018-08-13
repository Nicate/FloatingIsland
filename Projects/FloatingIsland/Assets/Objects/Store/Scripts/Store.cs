using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour {
	public float consumption = 0.0f;

	public float range = 1.0f;

	public float price = 0.0f;
	
	
	private void Start() {
		
	}
	

	private void Update() {
		
	}


	public void run(Manager manager) {
		// Find the closest silo with product available.
		float distance = float.MaxValue;
		Silo target = null;

		foreach(Silo silo in manager.getSilos()) {
			float currentDistance = Vector3.Magnitude(silo.transform.position - transform.position);

			if(currentDistance < distance && silo.hasProduct()) {
				distance = currentDistance;
				target = silo;
			}
		}

		float consumed = Mathf.Lerp(consumption, 0.0f, distance / range) * Time.deltaTime;

		// Suck 'er dry.
		if(target != null && consumed > 0.0f) {
			// If the target doesn't have enough product, the rest of the demand is wasted "as a penalty".
			float sold = target.withdraw(consumed);
			
			manager.funnel(sold * price);
		}
	}
}
