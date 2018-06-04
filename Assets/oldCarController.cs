using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldCarController : MonoBehaviour {
	 public float accelSpeed = 2f;
	 public float turnSpeed = 2f;
	 public int playerID = 1;

	 private Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		float movementX = Input.GetAxisRaw("Horizontal " + playerID);
		float movementY = Input.GetAxisRaw("Vertical " + playerID);
		Vector3 movSpd = movementX * accelSpeed * Vector3.forward;
		Quaternion turnSpd = Quaternion.identity;

		//transform.Translate(movSpd * Time.deltaTime);
		rigidbody.AddForce(movSpd);
	}
}
