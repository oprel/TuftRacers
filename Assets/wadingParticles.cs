using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wadingParticles : MonoBehaviour {

	public float waterLevel;
	public Vector2 waterRegion;
	private ParticleSystem.EmissionModule emission;
	private float rateOverDistance;

	void Start () {
		emission = GetComponent<ParticleSystem>().emission;
		rateOverDistance = emission.rateOverDistanceMultiplier;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.parent.position;
		if (pos.y<waterLevel+waterRegion.y && pos.y>waterLevel+waterRegion.x){
			emission.rateOverDistance=rateOverDistance;
		}else{
			emission.rateOverDistance=0;
		}
		pos.y=waterLevel;
		transform.position = pos;
	}
}
