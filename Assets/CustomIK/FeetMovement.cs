using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetMovement : MonoBehaviour {

	public Transform targetIK;
	public float legLength = 2;
	public float footSpeed = 3;

	private Vector3 targetPos;
	private Quaternion targetRot;
	private float legDelay;



	void PlaceLeg(){
		RaycastHit hit;
		Vector3 direction = (targetIK.position-transform.position);
		float distance = Vector3.Distance(targetIK.position,transform.position);
	
		Debug.DrawLine(transform.position, direction + transform.position, Color.red);
		int layerMask = LayerMask.GetMask("Floor");

		if (distance > legLength*2 || direction.y>0 || Physics.Raycast(transform.position, direction, out hit, distance, layerMask)){
			Debug.Log("HIT");
		}else if (legDelay >0 || distance < legLength)
				return;
		direction = (targetIK.position-transform.position) * -1;
		direction.y /=2;

		direction.y= Mathf.Abs(direction.y)*-1;
		direction += Vector3.down;
		//Debug.DrawLine(transform.position, transform.position + direction, Color.red);
		
		
		
		if (Physics.Raycast(transform.position, direction, out hit, legLength * 2, layerMask)){
			Vector3 random =  transform.forward * Random.Range(-1,1) + transform.right * Random.Range(-1,1);
			targetPos = hit.point + .0f * random;
	
				
		}else{
			targetPos=transform.position;
		}
			targetRot = transform.rotation;
			legDelay = Random.Range(0.0f,0.1f);
		
	}

	// Update is called once per frame
	void Update () {
		legDelay -=Time.deltaTime;
		Vector3 averagePos = new Vector3(0,0,0);
		PlaceLeg();
		targetIK.position = Vector3.Slerp(targetIK.position, targetPos, Time.deltaTime * footSpeed);
		targetIK.rotation = Quaternion.Slerp(targetIK.rotation, targetRot, Time.deltaTime * footSpeed);
		Vector3 stride = Vector3.Distance(targetIK.transform.position, targetPos) * Vector3.up;
		targetIK.position += stride * Time.deltaTime * footSpeed * .5f;


		averagePos += targetPos;
		averagePos.y=transform.position.y;
		transform.position = Vector3.Slerp(transform.position,averagePos,Time.deltaTime);


		//foot.transform.position = hit.point + .0f * random;
		//foot.transform.rotation = transform.rotation;

		
	}
}
