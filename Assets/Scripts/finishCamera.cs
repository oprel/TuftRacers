using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finishCamera : MonoBehaviour {


	public Vector3 middleOffset = new Vector3(0,5,0);
	public GameObject finishTile;
	public float speed;
	private Camera camera;

	void Start(){
		camera = GetComponent<Camera>();
	}
	void FixedUpdate () {
		if (gameManager.ending != null) return;
		transform.position = carManager.averagePos + middleOffset;
		Vector3 direction = finishTile.transform.position - transform.position;
 		Quaternion toRotation = Quaternion.LookRotation(direction);
 		transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.time);
	}
}
