using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class trackManager : MonoBehaviour {


	public static List<GameObject> checkpoints = new List<GameObject>();

	static int gridSize = 15;
	public static Vector3[,] grid = new Vector3[gridSize,gridSize];
	public static GameObject[,] placedTiles = new GameObject[gridSize,gridSize];
	public static List<GameObject> tileHistory = new List<GameObject>();

	private List<tile> checkedTiles = new List<tile>();

	private int updateTimer;
	private int checkpointCounter;

	public GameObject hexPrefab;
	//private GameObject[] tileObjs;
	private List<GameObject> tileObjs;

	private tile cursor = new tile(gridSize/2,gridSize/2);
	private int direction;
 
	
	public static float hexScale = 50;
    private Vector2 hexSize = new Vector2(hexScale,hexScale*Mathf.Sqrt(3)/2);
    public float gap = 0.0f;

	public struct tile{
		public int x;
		public int y;
	
		public tile(int a, int b){
			x = a;
			y = b;
		}
	}

	void CreateGrid(){
		for (int i = 0; i<gridSize; i++){
			for (int j = 0; j<gridSize; j++){
				grid[i,j] = new Vector3(i*hexSize.x,0,j*hexSize.y);
				if (j%2>0) grid[i,j].x += hexSize.x/2;
			}
		}
	}
	
	void CreateTile(){
		MoveCursor(direction);
		GameObject obj;
		int i =0;
		do{ obj = tileObjs[Random.Range(0,tileObjs.Count-1)];
		i++;
		if (i>100){
			Debug.Log("Endless Loop"); break;
		}
		}while (!checkPath(mod(direction+obj.GetComponent<trackTile>().directionDelta,6)));
		//Debug.Log( cursor.x + " : " + cursor.y + " is already placed: " +!checkPath(direction+obj.GetComponent<trackTile>().directionDelta));
		GameObject o = Instantiate(obj,grid[cursor.x,cursor.y],Quaternion.Euler(0,120+direction*60,0), transform);
		placedTiles[cursor.x,cursor.y] = o;
		o.name = "track " + cursor.x + ":" +cursor.y + " D:" + direction + " (" + obj.name +")";
		trackTile tile = o.GetComponent<trackTile>();
		direction = mod(direction+tile.directionDelta,6);
		tile.coordinates = cursor;
		checkpoint[] c = o.GetComponentsInChildren<checkpoint>();
		foreach (checkpoint p in c){
			checkpoints.Add(p.gameObject);
		}
	
	}

    void Start()
    {
		foreach (GameObject c in checkpoints){
			Destroy(c);
		}
		checkpoints.Clear();
		//tileObjs = Resources.LoadAll("Tiles") as GameObject[];
		tileObjs = new List<GameObject>(Resources.LoadAll<GameObject>("Tiles"));
		CreateGrid();
		
	/* 
		for (int i = 0; i<gridSize; i++){
			for (int j = 0; j<gridSize; j++){
				 placedTiles[i,j] = Instantiate(hexPrefab,grid[i,j],Quaternion.identity, transform);
			}
		}
		*/
		for (int i = 0; i<30; i++){
			CreateTile();
		}
		
	
    }

	// Update is called once per frame
	void Update () {

		foreach( GameObject checkpoint in checkpoints){
			if (!checkpoint) checkpoints.Remove(checkpoint);
		}

		foreach( GameObject checkpoint in GameObject.FindGameObjectsWithTag("checkpoint")){
			if (checkpoints.IndexOf(checkpoint) < 0){
				checkpoints.Add(checkpoint);
				checkpointCounter++;
				checkpoint.GetComponent<checkpoint>().id = checkpointCounter;
				checkpoint.name = "Checkpoint " + checkpointCounter;
			}
				;
		}
	}

	void MoveCursor(int d){
		switch (d){
			case 0:
				cursor.y++;
				if (cursor.y%2==0)cursor.x++;
				return;
			case 1:
				cursor.x++;
				return;
			case 2:
				cursor.y--;
				if (cursor.y%2==0)cursor.x++;
				return;
			case 3:
				cursor.y--;
				if (cursor.y%2==1)cursor.x--;
				return;
			case 4:
				cursor.x--;
				return;
			case 5:
				cursor.y++;
				if (cursor.y%2==1)cursor.x--;
				return;
		}
	}

	bool checkPath(int d){
		checkedTiles.Clear();
		tile current = cursor;
		MoveCursor(d);
		bool check = depthSearch(6);
		cursor = current;
		return check;
	}

	bool depthSearch(int depth){
		checkedTiles.Add(cursor);
		if (outOfBounds()) return false;
		if (placedTiles[cursor.x,cursor.y] != null) return false;
		if (depth>0){
			tile current = cursor;
			for (int i = 0; i<6; i++){
				cursor = current;
				MoveCursor(i);
				if (checkedTiles.Contains(cursor)) continue;
				bool b = depthSearch(depth-1);
				if (b) return b;
			}
		}
		return true;
	}

	bool outOfBounds(){
		return (cursor.x<0 || cursor.y<0 || cursor.x>gridSize-1 || cursor.y>gridSize-1);
	}

	void OnDrawGizmos() {
		CreateGrid();

		for (int i = 0; i<gridSize; i++){
			for (int j = 0; j<gridSize; j++){
				Gizmos.DrawWireSphere(grid[i,j],1);
			}
		}
        if (checkpoints.Count>2) {
			for (int i = 0; i<checkpoints.Count-1;i++){
				Vector3 current = checkpoints[i].transform.position;
				Vector3 next = checkpoints[i+1].transform.position;
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(current, next);
				Gizmos.DrawWireSphere(next,1);
				
				checkpoints[i].transform.LookAt(next);
				Gizmos.color = Color.green;
				Gizmos.DrawLine(current + checkpoints[i].transform.right,current-checkpoints[i].transform.right);
			}  
        }
    }

	int mod(int x, int m) {
		int r = x%m;
		return r<0 ? r+m : r;
	}

}
