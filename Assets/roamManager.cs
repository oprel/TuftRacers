using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roamManager : MonoBehaviour {
	public static roamManager self;
	public static bool roaming;
	public bool roamMode;
	public Transform nextRace, playerCar;
	private Camera cam;
	
	private void Start() {
		self = this;
		cam = Camera.main;
		setRoaming(roamMode);
	}

	// Update is called once per frame
	void Update () {
		if (roamMode != roaming) setRoaming(roamMode);
		if (roaming){
			cam.GetComponent<CameraFollower>().target = playerCar;
			carManager.cars[Random.Range(0,carManager.cars.Count)].GetComponent<carAI>().nextCheckpoint = nextRace.position;
		}

	}

	public static void setRoaming(bool roam){
		self.cam.GetComponent<CameraFollower>().enabled = roam;
		self.cam.GetComponent<CameraController>().enabled = !roam;
		roaming = roam;
	}
}
