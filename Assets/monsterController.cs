using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour {

	public Transform target;
	public float speed;
	[Range(0,1)]
	public float rotationSpeed;
	public Transform goal;
	public float dist;
	public float goalRadius;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//get target
		if (Random.value<.1f){
			float distance = Mathf.Infinity;
			foreach (carController car in carManager.cars){
				float d = Vector3.Distance(transform.position,car.transform.position);
				//get closest
				if (d<distance){
					//ignore if behind goal
					float angle = Vector3.Angle(transform.position-car.transform.position,transform.position-goal.position);
					if (!(d> Vector3.Distance(transform.position,goal.position) && angle<90)){
						distance = d;
						target = car.transform;
					}
				}
			}
		}
		//rotate
		rotate(target.position, rotationSpeed);
		dist = Vector3.Distance(transform.position,goal.transform.position);
		rotate(transform.position-goal.transform.position,1-dist/goalRadius);
		
		//move
		float s = Mathf.Clamp01(dist/goalRadius);
		s += 10*(1-Mathf.Clamp01(2*dist/goalRadius-.1f));
		transform.position+=transform.forward*s*speed;
	}

	void rotate(Vector3 target, float lerp){
		Quaternion rot = transform.rotation;
		transform.LookAt(target);
		rot = Quaternion.Lerp(rot,transform.rotation,lerp);
		rot.x=0;
		rot.z=0;
		transform.rotation = rot;
	}
	private void OnCollisionEnter(Collision other) {
		carController car = other.gameObject.GetComponent<carController>();
		if (car){
			car.Reset();
		}
	}
}
