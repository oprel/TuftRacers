using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour {


	public float DISTANCE_MARGIN = 1.0f;
	private float distanceBetweenPlayers;
	public float cameraDistance;
	public Vector3 rotation;
	private float aspectRatio;
	private float tanFov;
	private float magicZ = 1000000;
	private Camera cam;

	void Start() {
		cam = GetComponent<Camera>();
		aspectRatio = Screen.width / Screen.height;
		tanFov = Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView / 2.0f);
	}

	void Update () {
		// Position the camera in the center.
		Vector3 newCameraPos = transform.position;
		newCameraPos.x = carManager.averagePos.x;
		newCameraPos.z = -magicZ;
		transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rotation),.1f);
		transform.position = newCameraPos;


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
		transform.position = carManager.averagePos + dir * (cameraDistance + DISTANCE_MARGIN);
		cam.nearClipPlane = cameraDistance/3;
	}

	public float heightDiff(){
		return transform.position.y-carManager.averagePos.y;
	}
}
