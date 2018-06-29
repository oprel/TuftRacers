using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bumperPhysics : MonoBehaviour {
	[HideInInspector]
	public Rigidbody Rigidbody;
	public float bounce = 30;
	private Vector3 force;
	// Use this for initialization
	void Awake () {
		Rigidbody = GetComponent<Rigidbody>();
	}
	
	private void OnCollisionEnter(Collision collision) {
		bumperPhysics other =  collision.gameObject.GetComponent<bumperPhysics>();
		if (other){
			audioManager.playSFX(2);
			Vector3 impactVelocity = Rigidbody.velocity - other.Rigidbody.velocity;
			 foreach (ContactPoint contact in collision.contacts) {
				force = ( impactVelocity + contact.normal) * Rigidbody.mass * other.bounce;
				Rigidbody.AddForceAtPosition(force,contact.point);
       		}
		}
	}
}
