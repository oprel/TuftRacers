using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carBoost : MonoBehaviour {

    public int multiplierDuration = 60;
    private int multiplierTimer;
	private carController car;
	// Use this for initialization
	void Start () {
		car = GetComponent<carController>();
	}
	void FixedUpdate(){
		if (multiplierTimer>=0){
			float m = 1+Mathf.Clamp01(3*multiplierTimer/multiplierDuration);
			car.speedMultiplier= m;
			multiplierTimer--;
		}
	}
	public void applyBoost(){
		multiplierTimer = multiplierDuration;
	}
}
