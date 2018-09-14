using UnityEngine;

public class Factory : Building {
	[Header("Factory")]
	public float production = 0.0f;

	public float range = 1.0f;

	public float filthiness = 0.0f;


	public override void run(Manager manager) {
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

		float capacity = production * Time.deltaTime;

		float produced = Mathf.Lerp(capacity, 0.0f, distance / range);

		// Fill 'er up.
		if(target != null && produced > 0.0f) {
			// If the target doesn't have enough room, the rest is wasted "as a penalty".
			float deposited = target.deposit(produced);

			manager.pollute(deposited * filthiness);

			setRate(deposited / capacity);

			// We were able to produce.
			enableParticleSystems();
		}
		else {
			// We were unable to produce.
			disableParticleSystems();
		}
	}
}
