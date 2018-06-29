using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelParticles : MonoBehaviour {

	private ParticleSystem.EmissionModule particles;
	private WheelCollider WheelCollider;

	void Start(){
		WheelCollider = GetComponent<WheelCollider>();
		particles = GetComponent<ParticleSystem>().emission;
	}
	
	// Update is called once per frame
	void Update () {
		particles.enabled = WheelCollider.isGrounded;
	}
}
