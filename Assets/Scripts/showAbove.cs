using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showAbove : MonoBehaviour {

	private Vector3 offset;

	void Start(){
		offset = transform.position-transform.parent.transform.position;
	}
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.transform.position + offset;
	}
}
