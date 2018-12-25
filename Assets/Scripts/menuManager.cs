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
		HUD = UIManager.self.HUD;
		HUD.SetActive(false);
	}

	void FixedUpdate(){
		if (Input.GetKeyDown("escape"))
            Application.Quit();
	}
	
	public void StartGame(){
		gameManager = gameManager.self;
		HUD = UIManager.self.HUD;
		gameManager.carAmount = carSlider.i;
		gameManager.humanAmount = humanSlider.i;
		gameManager.aiGas = diffSlider.i/10f;
		HUD.SetActive(true);
		SceneManager.UnloadSceneAsync("menu");
		gameManager.init();
	}


}
