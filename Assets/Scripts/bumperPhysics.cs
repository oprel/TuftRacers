using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bumperPhysics : MonoBehaviour {
	[HideInInspector]
	public Rigidbody Rigidbody;
	public float bounce = 30;
	public float scaleChange = 1.2f;
	private Vector3 force;
	public int sound = 2;
	private Coroutine impact;
	// Use this for initialization
	void Awake () {
		Rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate() {
		
		/*Vector3 v = Rigidbody.velocity;
		v.y/=1.1f;
		Rigidbody.velocity = v;
		*/
	}
	
	private void OnCollisionEnter(Collision collision) {
		bumperPhysics other =  collision.gameObject.GetComponent<bumperPhysics>();
		if (other){
			if (impact == null) impact = StartCoroutine(scaleCol(collision));
			audioManager.playSFX(other.sound);
			return;
			
			Vector3 impactVelocity = Rigidbody.velocity - other.Rigidbody.velocity;
			 foreach (ContactPoint contact in collision.contacts) {
				force = ( impactVelocity + contact.normal) * Rigidbody.mass * other.bounce;
				Rigidbody.AddForceAtPosition(force,contact.point);
       		}
		}
	}

	private IEnumerator scaleCol(Collision col){
		CapsuleCollider b= col.gameObject.GetComponent<CapsuleCollider>();
		if (b){
			b.radius *=scaleChange;
			yield return new WaitForSeconds(.1f);
			b.radius /=scaleChange;
			
		}
		impact = null;

	}
}
