using UnityEngine;

public class Building : MonoBehaviour {
	public void enableParticleSystems() {
		foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()) {
			if(!particleSystem.isPlaying) {
				particleSystem.Play();
			}
		}
	}
	
	public void disableParticleSystems() {
		foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()) {
			if(particleSystem.isPlaying) {
				particleSystem.Stop();
			}
		}
	}


	public void enableRadiators() {
		foreach(Radiator radiator in GetComponentsInChildren<Radiator>(true)) {
			radiator.gameObject.SetActive(true);
		}
	}
	
	public void disableRadiators() {
		foreach(Radiator radiator in GetComponentsInChildren<Radiator>(true)) {
			radiator.gameObject.SetActive(false);
		}
	}


	public virtual void evacuate() {
		disableParticleSystems();
		disableRadiators();
	}
}
