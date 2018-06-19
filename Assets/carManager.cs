using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager : MonoBehaviour {

	public static List<carController> cars = new List<carController>();
	public static Vector3 averagePos;
	public static float carWidth = 2;
	public static float stuckTimer = 2;
	public static GameObject playerInLead;
	public static int leadCounter = 0;
	public static int carsInPlay = cars.Count;
	public static GameObject leadTile;
	private static Coroutine ending;

	private trackManager trackManager;

	void Start(){
		trackManager = GetComponent<trackManager>();
		Reset();
		
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
		if (ending == null && playerInLead && carsInPlay<2){
			ending = StartCoroutine(Winner(playerInLead.GetComponent<carController>().playerID));
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
			car.lastCheckpoint = trackManager.checkpoints[0];
			car.Reset();
		}
	}

	public static void resetCars(){
		foreach (carController car in cars){
			//car.Reset();
			car.gameObject.SetActive(true);
			Debug.Log("should be active bro");
		}
	}

	public static IEnumerator Winner(int id){
		UIManager.self.showText("PLAYER " + id + " WINS");
		Debug.Log("Winner");
		yield return new WaitForSeconds(2.2f);
		foreach (carController car in cars){
			car.gameObject.SetActive(true);
		}
		trackManager.self.Clear();
	}
	

}
