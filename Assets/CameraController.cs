using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	public float DISTANCE_MARGIN = 1.0f;
	private float distanceBetweenPlayers;
	private float cameraDistance;
	private float aspectRatio;
	private float tanFov;
	private float magicZ = 1000000;

	void Start() {
		aspectRatio = Screen.width / Screen.height;
		tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
	}

	void Update () {
		// Position the camera in the center.
		Vector3 newCameraPos = Camera.main.transform.position;
		newCameraPos.x = carManager.averagePos.x;
		newCameraPos.z = -magicZ;

		Camera.main.transform.position = newCameraPos;


		// Calculate the new distance.
		distanceBetweenPlayers = 0;
		foreach (carController car in carManager.cars){
			if (!car.gameObject.activeInHierarchy) continue;
			float d = Vector3.Distance(car.transform.position,carManager.averagePos);
			if (d > distanceBetweenPlayers) distanceBetweenPlayers = d;
		}
		cameraDistance = (distanceBetweenPlayers / aspectRatio) / tanFov;

		// Set camera to new position.
		Vector3 dir = transform.rotation * (Camera.main.transform.position - carManager.averagePos).normalized;
		Camera.main.transform.position = carManager.averagePos + dir * (cameraDistance + DISTANCE_MARGIN);
	}
}
