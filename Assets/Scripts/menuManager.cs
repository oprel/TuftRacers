using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class menuManager : MonoBehaviour {
	private GameObject HUD;
	private gameManager gameManager;
	public intSlider carSlider;
	public intSlider humanSlider;
	public intSlider diffSlider;

	

	// Use this for initialization
	void Start () {
		gameManager = gameManager.self;
		//HUD = UIManager.self.HUD;
		//HUD.SetActive(false);
		StartCoroutine(loadScene("terrainSmall"));
		
	}

	void FixedUpdate(){
		if (Input.GetKeyDown("escape"))
            Application.Quit();
	}
	
	public void StartGame(){
		StartCoroutine(startingGame());
	}

	public IEnumerator startingGame(){
		StartCoroutine(loadScene("main"));
		do{
			gameManager = gameManager.self;
			yield return null;
		}while(!gameManager);
		
		gameManager.carAmount = carSlider.i;
		gameManager.humanAmount = humanSlider.i;
		gameManager.aiGas = diffSlider.i/10f;
		HUD = UIManager.self.HUD;
		HUD.SetActive(true);
		
		SceneManager.UnloadSceneAsync("menu");
		gameManager.newRound();
	}

	IEnumerator loadScene(string name){
		SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
		Scene scene = SceneManager.GetSceneByName(name);
		do{
			yield return null;
		}while(!scene.isLoaded);
		
		SceneManager.SetActiveScene(scene);
	}


}
