using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class tilepileSpawner : MonoBehaviour {

	public static List<GameObject> tiles = new List<GameObject>();
	public float force = 5;
	public float scale;
	private float timer = 0;
	public Vector3 spawnpoint;
	// Use this for initialization
	void Start () {
		foreach (GameObject obj in new List<GameObject>(Resources.LoadAll<GameObject>("Tiles"))){
			tiles.Add(obj);
		};
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.value > .9f){
			Destroy(spawnTile(),1f);
		}
		
	}

	public GameObject spawnTile(){
		GameObject prefab = tiles[Random.Range(0,tiles.Count)];
		GameObject tile = Instantiate(prefab, transform);
		
		tile.transform.localScale = Vector3.one * scale;
		tile.transform.position = randomPos();
		MeshCollider c = tile.GetComponent<MeshCollider>();
		
		if (!c)
			c = tile.AddComponent<MeshCollider>();
		c.convex = true;
		Rigidbody rb = tile.AddComponent<Rigidbody>();
		rb.AddForce(Random.insideUnitSphere * force);
		return tile;
	}

	private Vector3 randomPos(){
		Vector3 o = new Vector3(Random.Range(0,spawnpoint.x),Random.Range(0,spawnpoint.y),Random.Range(0,spawnpoint.z));
		o = transform.rotation* (o - spawnpoint/2);
		return transform.position + o;
	}

	private void OnDrawGizmosSelected() {
		Gizmos.DrawWireCube(transform.position, spawnpoint);
	}
}
