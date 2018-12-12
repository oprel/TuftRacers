using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterManager : MonoBehaviour {

	static monsterManager self;
	public GameObject monsterPrefab;
	public int maxMonsters = 8;
	public float timeBetweenSpawns = 5;
	public static List<GameObject> monsters = new List<GameObject>();
	private static Transform avoid;
	public float spawnRadius = 200;
	

	private void Awake() {
		self = this;
	}
	private void Update() {
		if (!avoid) avoid = trackManager.self.finishTile.transform;
	}

	public static void newRound(){
		self.StopAllCoroutines();
		foreach (GameObject monster in monsters){
			Destroy(monster);
		}
		monsters.Clear();
		self.StartCoroutine(spawnMonsters());
	}
	
	private static IEnumerator spawnMonsters(){
		for (int i = 0; i < self.maxMonsters;)
		{
			if (avoid){
				spawnMonster();
				i++;
			}
			yield return new WaitForSeconds(self.timeBetweenSpawns);
		}
		
	}

	public static void spawnMonster(){
		GameObject o = Instantiate(self.monsterPrefab);
		Vector3 ran = Random.insideUnitSphere * self.spawnRadius;
		ran.y=avoid.position.y;
		o.transform.position = avoid.position + ran;
		o.GetComponent<monsterController>().avoid = avoid;
		monsters.Add(o);
	}
}
