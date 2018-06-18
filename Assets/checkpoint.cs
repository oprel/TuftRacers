using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class checkpoint : MonoBehaviour {

	public int id;

	public Vector3 NextCheckpoint(){
		int current = trackManager.checkpoints.IndexOf(gameObject);
		int next = current+1;
		if (current == trackManager.checkpoints.Count-1)
			next = 0;
		return trackManager.checkpoints[next].transform.position;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag=="Player"){
			other.GetComponent<carController>().lastCheckpoint=gameObject;
			UpdateLead(other.gameObject);

			carAI ai = other.GetComponent<carAI>();
			if (ai){
				ai.nextCheckpoint = NextCheckpoint();
			}
		}
	}

	void UpdateLead(GameObject player){
		if (id>carManager.leadCounter){
			carManager.leadCounter=id;
			carManager.playerInLead=player;
		}
	}
}
