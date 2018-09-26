using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class supportBuilder : MonoBehaviour {

	public static supportBuilder self;
	//public static Vector3[,] grid = new Vector3[gridSize,gridSize];
	public static List<Vector3> supportLocations= new List<Vector3>();
	public static List<GameObject> placedSupports= new List<GameObject>();
	public GameObject supportPrefab;
	private static GameObject obj;

	private void Awake() {
		self = this;
	}

	private static Vector3 getCorner(Vector3 center, int i){
		float angle_deg = -60 * i -30;
		float angle_rad = Mathf.PI / 180 * angle_deg;
		float size = trackManager.hexScale/2 + 3.75f;
		return new Vector3(	(int)center.x + size * Mathf.Cos(angle_rad), 
							center.y,
            				(int)center.z + size * Mathf.Sin(angle_rad));
	}
    

	public static void buildSupports(Vector3 tile, int direction, GameObject o){
		for (int i = direction-2; i < direction; i++)
		{
			Vector3 corner = getCorner(tile,i);
			if (supportLocations.Contains(corner)) return;
			supportLocations.Add(corner);
			obj = Instantiate(self.supportPrefab, self.transform);
			obj.transform.position = corner;
			obj.transform.SetParent(o.transform);
			placedSupports.Add(obj);
		}
		
	}
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		foreach (Vector3 p in supportLocations)
		{
			Gizmos.DrawWireSphere(p,1);
		}
	}

	public static void Clear(){
		foreach (GameObject o in placedSupports){
			Destroy(o);
		}
		placedSupports.Clear();
		supportLocations.Clear();
	}
}
