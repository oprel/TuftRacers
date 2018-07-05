using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostPad : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		carBoost boost = other.GetComponent<carBoost>();
		if (boost)
		{
			boost.applyBoost();
			audioManager.playSFX(5);
		}
	}

}
