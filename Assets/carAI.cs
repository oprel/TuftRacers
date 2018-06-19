using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carAI : MonoBehaviour {

	[HideInInspector]
	public Vector3 nextCheckpoint;
	private carController carController;
	public float gas = .5f;

	// Use this for initialization
	void Awake () {
		carController = GetComponent<carController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 relativeVector = transform.InverseTransformPoint(nextCheckpoint);
		float steering = (relativeVector.x / relativeVector.magnitude) * carController.maxSteeringAngle;
		float motor = carController.maxMotorTorque*gas;
		carController.applyWheels(motor,steering);
	}

	public void setNextTarget(Transform t){
		nextCheckpoint = t.position;
		nextCheckpoint += carController.carOffset(t);
	}

}
