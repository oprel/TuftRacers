using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour {

	public Transform target;
	public float speed;
	[Range(0,.2f)]
	public float rotationSpeed;
	
	public float dist;
	public Transform avoid;
	public float avoidRadius;
	public float heightOffset;

	private legPlacer[] legs;
	
	// Use this for initialization
	void Start () {
		legs = GetComponentsInChildren<legPlacer>();
	}
	
	// Update is called once per frame
	void Update () {
		float h=0;
		foreach (legPlacer leg in legs)
		{
			h+=leg.desiredHeight;
		}
		Vector3 pos = transform.position;
		pos.y = Mathf.Lerp(pos.y,h/legs.Length+heightOffset,.1f);
		transform.position = pos;

		//get target
		if (Random.value<.1f){
			float distance = Mathf.Infinity;
			foreach (carController car in carManager.cars){
				float d = Vector3.Distance(transform.position,car.transform.position);
				//get closest active car
				if (car.isActiveAndEnabled && d<distance){
					//ignore if behind avoid
					float angle = Vector3.Angle(transform.position-car.transform.position,transform.position-avoid.position);
					if (!(d> Vector3.Distance(transform.position,avoid.position) && angle<90)){
						distance = d;
						target = car.transform;
					}
				}
			}
		}
		if (target){
		//rotate
		rotate(target.position, rotationSpeed);
		dist = Vector3.Distance(transform.position,avoid.transform.position);
		rotate(transform.position-avoid.transform.position,1-dist/avoidRadius);
		
		//move
		float s = Mathf.Clamp01(dist/avoidRadius);
		s += 10*(1-Mathf.Clamp01(2*dist/avoidRadius-.1f));
		transform.position+=transform.forward*s*speed;
		}
		
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
			car.Reset(true);
		}
	}
}
