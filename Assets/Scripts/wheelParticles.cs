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
		//if (WheelCollider.rpm>100) Debug.Log(WheelCollider.rpm);
		particles.enabled = WheelCollider.isGrounded;
		float a = Mathf.Clamp(Mathf.Abs((WheelCollider.rpm+WheelCollider.motorTorque)/1500),.1f,5);
		particles.rateOverDistance = a;
		//particles.rateOverDistance = new ParticleSystem.MinMaxCurve(amount/2,amount);
		
		
	}
}
