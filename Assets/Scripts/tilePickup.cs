using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tilePickup : MonoBehaviour {

	public Texture2D[] sprites;
	public static Material[] materials = new Material[1];
	public int direction;
	public GameObject sourcePlayer;
	// Use this for initialization
	void Start () {
		if (materials.Length!=sprites.Length) materials = new Material[sprites.Length];
		
		//get optimal tile
		int c = 0;
		do{
			direction = Random.Range(-2,2);
			c++;
		}while(!trackManager.self.validPath(direction) && c<10);
		
		int i = direction+2;
		Renderer renderer = GetComponent<Renderer>();
		if (!materials[i]){
			Material mat = new Material(renderer.material.shader);
			mat.CopyPropertiesFromMaterial(renderer.material);
			mat.SetTexture("_MainTex",sprites[i]);
			materials[i] = mat;
		}
		renderer.material = materials[i];

	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag=="Player" && other.gameObject != sourcePlayer){
			audioManager.playSFX(0);
			trackManager.self.newTile(direction);
			Destroy(gameObject);
		}

	}
}
