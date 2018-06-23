using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {

	public static gameManager self;
	public int roundsToWin;
	public GameObject carPrefab;
	public int carAmount;
	public int AIAmount;
	public Material[] carColors;
	public static Coroutine ending;

	void Start () {
		self = this;
		SpawnCars();
		newRound();
		UIManager.updateScoreDisplay();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnCars(){
		List<carController> cars = new List<carController>();
		for (int i = 0; i<carAmount;i++){
			GameObject o = Instantiate(carPrefab);
			carController car = o.GetComponent<carController>();
			if (i<carAmount-AIAmount)o.GetComponent<Renderer>().material = carColors[i%carColors.Length];
			car.playerID = i+1;
			car.order=i;
			o.name="Player "+ car.playerID;
			o.GetComponent<carAI>().enabled = i>=carAmount-AIAmount;
			
			cars.Add(car);
		}
		carManager.cars=cars;
	}

	public static void newRound(){
		trackManager.self.Clear();
		carManager.Reset();
		UIManager.updateScoreDisplay();

	}

	public static void Winner(carController car){
		if (ending ==null) ending = self.StartCoroutine(endRound(car));
	}

	public static IEnumerator endRound(carController winner){
		winner.wins++;
		if (winner.wins>=self.roundsToWin){
			UIManager.self.showText("PLAYER " + winner.playerID + " WINS THE ENTIRE GAME");
			yield return new WaitForSeconds(8.2f);
		}

		UIManager.self.showText("PLAYER " + winner.playerID + " WINS");
		
		UIManager.updateScoreDisplay();
		yield return new WaitForSeconds(3.2f);
		foreach (carController car in carManager.cars){
			car.gameObject.SetActive(true);
		}
		gameManager.newRound();
		ending=null;
	}

}
