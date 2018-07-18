using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particlesAutoDestroy : MonoBehaviour {

	private ParticleSystem ps;
	void Awake(){
		ps = GetComponent<ParticleSystem>();
	}
	void Update () {
		if (ps && !ps.isPlaying) Destroy(gameObject);
	}
}
