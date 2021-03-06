﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class checkpoint : MonoBehaviour {

	public int id;
	public GameObject parentTile;
	public static float size =15;

	public void Init(int i, GameObject p){
		id = i;
		parentTile = p;
		GetComponent<SphereCollider>().radius = size;
	}
	
	public Transform NextCheckpoint(){
		int current = trackManager.checkpoints.IndexOf(gameObject);
		int next = current+1;
		if (current == trackManager.checkpoints.Count-1)
			next = 0;
		if (!trackManager.checkpoints[next]) return null;
		return trackManager.checkpoints[next].transform;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag=="Player"){
			other.GetComponent<carController>().lastCheckpoint=gameObject;
			UpdateLead(other.gameObject);

			carAI ai = other.GetComponent<carAI>();
			if (ai){
				ai.setNextTarget(NextCheckpoint());
			}
		}
	}

	void UpdateLead(GameObject player){
		if (id>carManager.leadCounter){
			carManager.leadCounter=id;
			carManager.playerInLead=player.GetComponent<carController>();
			carManager.leadTile = parentTile;
			trackManager.checkpointBuild(transform.parent.gameObject);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color=Color.blue;
		Gizmos.DrawSphere(transform.position,.6f);
	}
}
