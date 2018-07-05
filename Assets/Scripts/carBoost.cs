using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carBoost : MonoBehaviour {

    public float multiplierDuration = 10;
	public GameObject boostDisplay;
    private float multiplierTimer;
	private carController car;


	// Use this for initialization
	void Awake () {
		car = GetComponent<carController>();
		car.ResetAll += Reset;
	}
	void FixedUpdate(){
		boostDisplay.SetActive(multiplierTimer>0);
		if (multiplierTimer>=0){
			float m = 1+Mathf.Clamp01(3*multiplierTimer/multiplierDuration);
			car.speedMultiplier= m;
			multiplierTimer-= Time.deltaTime ;
		}
	}
	public void applyBoost(){
		multiplierTimer = multiplierDuration;
	}

	private void Reset() {
		multiplierTimer=0;
	}
}
