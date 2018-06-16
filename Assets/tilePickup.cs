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

		direction = Random.Range(-2,2);
		int i = direction+2;
		Renderer renderer = GetComponent<Renderer>();
		if (!materials[i]){
			Material mat = new Material(renderer.material.shader);
			renderer.material.CopyPropertiesFromMaterial(mat);
			mat.SetTexture("_MainTex",sprites[i]);
			materials[i] = mat;
		}
		renderer.material = materials[i];

	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag=="Player" && other.gameObject != sourcePlayer){
			trackManager.self.newTile(direction);
			Destroy(gameObject);
		}

	}
}
