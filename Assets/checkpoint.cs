using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {

	public int id;

	public Vector3 NextCheckpoint(){
		int current = trackManager.checkpoints.IndexOf(gameObject);
		int next = current+1;
		if (current == trackManager.checkpoints.Count)
			next = 0;
		return trackManager.checkpoints[next].transform.position;
	}

	void OnTriggerEnter(Collider other){
		Debug.Log("hit");
		if (other.tag=="Player"){
			other.GetComponent<carController>().lastCheckpoint=gameObject;
		}
	}
}
