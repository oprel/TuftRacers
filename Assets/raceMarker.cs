﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raceMarker : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		if (other.tag=="Player" && !other.GetComponent<carAI>().enabled){
			trackManager.setStart(transform.position);
			roamManager.startRace();
		}
	}
}