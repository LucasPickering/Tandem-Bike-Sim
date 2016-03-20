using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {

	void Start () {
		ParticleSystem pSystem = GetComponent<ParticleSystem>();
		pSystem.Play();
		Destroy(gameObject, pSystem.duration);
	}
}
