using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour {

	public static gameManager self;
	public enum gameTypes
	{
		TRACKCULLING,
		MONSTERCULLING,
		INFINITE
	}
	public gameTypes gameType;
	public int roundsToWin;
	public GameObject carPrefab;
	public int carAmount;
	public int humanAmount;
	public Material[] carColors;
	public static Coroutine ending;
	public GameObject pickup;
	public float aiGas = .5f;
	public float timeScale = 1.5f;

	private int update;

	void Awake(){
		self = this;
		SceneManager.LoadSceneAsync("env",LoadSceneMode.Additive);
	}

	void Start(){
		openMenu();
	}

	public void init () {
		SpawnCars();
		newRound();
		audioManager.nextBGM();
		

	}

	public void openMenu(){
		SceneManager.LoadSceneAsync("menu",LoadSceneMode.Additive);
		carAmount=4;
		humanAmount=0;
		init();
	}
	
	// Update is called once per frame
	void Update () {
		Time.timeScale=timeScale;
		if (Input.GetKeyDown("escape"))
            openMenu();


		update++;
		if (update>240){
			update=0;
			GameObject lead = carManager.playerInLead.gameObject;
			if (lead){
				return;
				GameObject o = Instantiate(pickup,lead.transform.position + Vector3.up,Quaternion.identity);
				o.GetComponent<tilePickup>().sourcePlayer = lead;
			}
			//newTile(Random.Range(-2,3));
			//if (tileHistory.Count>200) Clear();
			//Debug.ClearDeveloperConsole();
			
		}
	}

	void SpawnCars(){
		foreach (carController car in carManager.cars){
			if (car.gameObject)Destroy(car.gameObject);
		}
		
		List<carController> cars = new List<carController>();
		for (int i = 0; i<carAmount;i++){
			GameObject o = Instantiate(carPrefab);
			carController car = o.GetComponent<carController>();
			if (i<humanAmount)o.GetComponent<Renderer>().material = carColors[i%carColors.Length];
			car.playerID = i+1;
			car.order=i;
			o.name="Player "+ car.playerID;
			o.GetComponent<carAI>().enabled = i>=humanAmount;
			cars.Add(car);
		}
		carManager.cars=cars;
	}

	public static void newRound(){
		trackManager.self.Clear();
		carManager.Reset();
		UIManager.updateScoreDisplay();
		if (self.gameType == gameTypes.MONSTERCULLING) monsterManager.newRound();

	}

	public static void Winner(carController car){
		if (ending ==null) ending = self.StartCoroutine(endRound(car));
	}

	public static IEnumerator endRound(carController winner){
		winner.wins++;
		audioManager.playSFX(1);
		if (winner.wins>self.roundsToWin){
			UIManager.self.showText("PLAYER " + winner.playerID + " WINS THE ENTIRE GAME");
			yield return new WaitForSeconds(8.2f);
			ending=null;
			self.openMenu();
		}else{
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

}
