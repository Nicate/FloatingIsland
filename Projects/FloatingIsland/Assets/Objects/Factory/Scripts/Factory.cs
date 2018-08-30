using UnityEngine;

public class Factory : Building {
	[Header("Factory")]
	public float production = 0.0f;

	public float range = 1.0f;

	public float filthiness = 0.0f;


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
			enableParticleSystems();
		}
		else {
			// We were unable to produce.
			disableParticleSystems();
		}
	}
}
