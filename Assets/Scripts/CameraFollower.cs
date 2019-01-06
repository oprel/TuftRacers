using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollower : MonoBehaviour {


	public float DISTANCE_MARGIN = 1.0f;
	public Vector3 offset;
	public Transform target;
	public float cameraDistance;
	[Range(0,1)]
	public float speed;
	private float aspectRatio;
	private float tanFov;
	private Rigidbody rb;
	private Camera cam;

	void Start() {
		if (!target) target = carManager.cars[0].transform;
		aspectRatio = Screen.width / Screen.height;
		tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
		rb = target.GetComponent<Rigidbody>();
		cam = GetComponent<Camera>();
		
	}

	void Update () {
		if (!target) target = carManager.cars[0].transform;
		cam.nearClipPlane=.001f;
		// Position the camera in the center.
		Vector3 targetCameraPos = target.position + target.rotation * offset;
		if (rb){
				targetCameraPos.y -= Mathf.Clamp01((rb.velocity.magnitude-20)/10)*20;
				targetCameraPos.z += Mathf.Clamp01((rb.velocity.magnitude-20)/10)*20;
		}
		
		cam.transform.position = Vector3.Lerp(cam.transform.position,targetCameraPos,speed);
		cam.transform.LookAt(target);
		if (rb)
		cameraDistance = (rb.velocity.magnitude / aspectRatio) / tanFov;


	}
}
