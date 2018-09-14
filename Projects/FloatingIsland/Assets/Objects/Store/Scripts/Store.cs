using UnityEngine;

public class Store : Building {
	[Header("Store")]
	public float consumption = 0.0f;

	public float range = 1.0f;

	public float price = 0.0f;


	public override void run(Manager manager) {
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

		float capacity = consumption * Time.deltaTime;

		float consumed = Mathf.Lerp(capacity, 0.0f, distance / range);

		// Suck 'er dry.
		if(target != null && consumed > 0.0f) {
			// If the target doesn't have enough product, the rest of the demand is wasted "as a penalty".
			float sold = target.withdraw(consumed);
			
			manager.funnel(sold * price);

			setRate(sold / capacity);

			// We were able to consume.
			enableParticleSystems();
		}
		else {
			// We were unable to consume.
			disableParticleSystems();
		}
	}
}
