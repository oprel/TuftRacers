using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carAI : MonoBehaviour {

	[HideInInspector]
	public Vector3 nextCheckpoint;
	public Material AIColor;
	private carController carController;
	public float gas = .5f;

	// Use this for initialization
	void Start () {
		carController = GetComponent<carController>();
		GetComponent<Renderer>().material = AIColor;
		gas = gameManager.self.aiGas;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float s = carManager.cars.Count;

		float g = gas - .05f *(s/2 - s*carController.order);
		Vector3 relativeVector = transform.InverseTransformPoint(nextCheckpoint);
		float steering = (relativeVector.x / relativeVector.magnitude) * carController.maxSteeringAngle;
		float motor = carController.maxMotorTorque*g;
		carController.applyWheels(motor,steering);
		
	}

	public void setNextTarget(Transform t){
		if (!t || !carController) return;
		nextCheckpoint = t.position;
		nextCheckpoint += carController.carOffset(t);
	}

}
