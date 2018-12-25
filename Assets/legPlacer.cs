using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class legPlacer : MonoBehaviour {
	public Transform targetIK;
	

	public float legLength;
	public float minLegHeight, movementSpeed, maxLegAngle, footOffset;
	public Vector3 restingPlace;
	public AnimationCurve heightCurve;
	public float heightMultiplier;
	[HideInInspector]
	public float desiredHeight;
	

	private float maxLegDistance;
	private float legTimer;
	private Vector3 targetPos, oldPos;
	private Quaternion targetRot, oldRot;
	private bool legPlaced;
	void Start () {
		placeLeg();
	}

	void placeLeg(){
		//Vector3 direction = new Vector3(0,-legLength,0);
		Vector3 direction = dir();
		
		RaycastHit hit;
		legPlaced = Physics.Raycast(transform.position, direction, out hit, legLength) && Vector3.Distance(hit.point,transform.position)>minLegHeight;
		if (legPlaced){
			legTimer=0;
			oldPos = targetIK.position;
			oldRot = targetIK.rotation;
			targetPos=hit.point + hit.normal * footOffset;
			//align with normal
			targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			//rotate 90 degrees
			//Vector3 rot = Quaternion.LookRotation(hit.normal).eulerAngles;
			//targetRot *= Quaternion.EulerAngles(rot.x,0,rot.z);
			//targetRot *= Quaternion.FromToRotation( Vector3.left, Vector3.forward);
		}
	}
	// Update is called once per frame
	void Update () {
		
		stabilize();
		if (legPlaced){
			legTimer+=Time.deltaTime;
			float l = Mathf.Clamp01(legTimer/movementSpeed);
			targetIK.position = Vector3.Lerp(oldPos,targetPos,l) + new Vector3(0,heightCurve.Evaluate(l)*heightMultiplier,0);
			targetIK.rotation = Quaternion.Lerp(oldRot, targetRot, l);
			if (Vector3.Distance(transform.position,targetPos)>(legLength)/Mathf.Cos(maxLegAngle * Mathf.Deg2Rad)){
				placeLeg();
			}
		}else{
			targetIK.position = Vector3.Lerp(targetIK.position,transform.position+ transform.rotation* restingPlace,.1f);
			placeLeg();
		}
		
	}

	void stabilize(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -Vector3.up, out hit)){
			desiredHeight =  hit.point.y+legLength/2;
		}
	}

	Vector3 dir(){
		Vector3 down = new Vector3(0,-legLength,0);
		if (!legPlaced) return down;
		Vector3 direction = (targetIK.position-transform.position);
		//direction = Vector3.Reflect(direction, reflectNormal);
		direction = (targetIK.position-transform.position) * -1;
		direction.y= Mathf.Abs(direction.y)*-1;
		direction+=down;
		Debug.DrawLine(transform.position, direction + transform.position, Color.red);
		return direction;
	}

	private void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(targetPos,1);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(targetIK.position,.7f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position-new Vector3(0,legLength,0));
	}
}
