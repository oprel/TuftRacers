using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTile : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.tag=="Player"){
			gameManager.Winner(other.GetComponent<carController>());
		}
	}
}
