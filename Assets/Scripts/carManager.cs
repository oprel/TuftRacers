using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager : MonoBehaviour {

	public static List<carController> cars = new List<carController>();
	public static Vector3 averagePos;
	public static float carWidth = 2.5f;
	public static float stuckTimer = .7f;
	public static carController playerInLead;
	public static int leadCounter = 0;
	public static int carsInPlay = cars.Count;
	public static GameObject leadTile;
	public static Coroutine ending;

	private trackManager trackManager;

	void Awake(){
		trackManager = GetComponent<trackManager>();
		Reset();
		GameObject[] findcars = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject findcar in findcars){
			carController car = findcar.GetComponent<carController>();
			if (car && !cars.Contains(car)){
				cars.Add(car);
			}
		}
	}

	public static void Reset(){
		leadCounter = 0;
		playerInLead = null;
		ending = null;
		resetCars();
	}


	
	// Use this for initialization
	private void FindAveragePosition()
		{
			Vector3 pos = new Vector3();
			int numTargets = 0;
			
			for (int i = 0; i < cars.Count; i++)
			{
				carController car = cars[i];
				if (!car.gameObject.activeInHierarchy)
					continue;

				pos += car.transform.position;
				pos += car.rigidbody.velocity.magnitude/3  * car.transform.forward;
				numTargets++;
			}
			carsInPlay = numTargets;

			if (numTargets > 0)
				pos /= numTargets;
			averagePos=Vector3.Lerp(averagePos,pos,.08f);
		}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerInLead && carsInPlay<2){
			gameManager.Winner(playerInLead);
		}

		FindAveragePosition();
		hasCheckpoint();
		Shader.SetGlobalVector("_DissolvePosition", averagePos);
		if (Random.value>.9f){
			foreach (carController car in cars){
				carController car2 = cars[Random.Range(0,cars.Count)];
				int i = car.order;
				car.order = car2.order;
				car2.order = i;
			}
		}
	}

	void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(averagePos, 2f);
    }

	void hasCheckpoint(){
		foreach (carController car in cars){
			if ( !car.gameObject.activeInHierarchy ||car.lastCheckpoint || trackManager.checkpoints.Count<1) continue;
			car.lastCheckpoint = trackManager.checkpoints[1];
			car.Reset();
		}
	}

	public static void resetCars(){
		foreach (carController car in cars){
			car.Reset();
			car.gameObject.SetActive(true);
		}
	}

	
	

}
