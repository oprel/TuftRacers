using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delayEnable : MonoBehaviour {

	public float delayTime;
	private float timer;
	// Use this for initialization
	void Start () {
		foreach (Transform child in transform){
			child.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer+=Time.deltaTime;
		if (timer>delayTime){
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
			}
			this.enabled=false;
		}
		
	}

}
