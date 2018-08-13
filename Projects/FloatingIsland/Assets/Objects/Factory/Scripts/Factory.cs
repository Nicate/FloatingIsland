using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour {
	public float production = 0.0f;

	public float range = 1.0f;

	public float filthiness = 0.0f;


	private void Start() {
		foreach(ParticleSystem particleSystem in transform.GetComponentsInChildren<ParticleSystem>()) {
			particleSystem.Stop();
		}
	}
	

	private void Update() {
		
	}


	public void run(Manager manager) {
		// Find the closest silo with room available.
		float distance = float.MaxValue;
		Silo target = null;

		foreach(Silo silo in manager.getSilos()) {
			float currentDistance = Vector3.Magnitude(silo.transform.position - transform.position);

			if(currentDistance < distance && silo.hasRoom()) {
				distance = currentDistance;
				target = silo;
			}
		}

		float produced = Mathf.Lerp(production, 0.0f, distance / range) * Time.deltaTime;

		// Fill 'er up.
		if(target != null && produced > 0.0f) {
			// If the target doesn't have enough room, the rest is wasted "as a penalty".
			float deposited = target.deposit(produced);

			manager.pollute(deposited * filthiness);

			// We were able to produce.
			foreach(ParticleSystem particleSystem in transform.GetComponentsInChildren<ParticleSystem>()) {
				if(!particleSystem.isPlaying) {
					particleSystem.Play();
				}
			}
		}
		else {
			// We were unable to produce.
			foreach(ParticleSystem particleSystem in transform.GetComponentsInChildren<ParticleSystem>()) {
				if(particleSystem.isPlaying) {
					particleSystem.Stop();
				}
			}
		}
	}
	
	
	public void stopSmoking() {
		foreach(ParticleSystem particleSystem in transform.GetComponentsInChildren<ParticleSystem>()) {
			particleSystem.Stop();
		}
	}
}
