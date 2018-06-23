using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class trackManager : MonoBehaviour {

	public static trackManager self;
	public static List<GameObject> checkpoints = new List<GameObject>();

	static int gridSize = 50;
	public static Vector3[,] grid = new Vector3[gridSize,gridSize];
	public static GameObject[,] placedTiles = new GameObject[gridSize,gridSize];
	public static List<GameObject> tileHistory = new List<GameObject>();
	private List<tile> checkedTiles = new List<tile>();

	private int updateTimer;
	private int checkpointCounter;

	public GameObject hexPrefab;
	//private GameObject[] tileObjs;
	private List<List<GameObject>> tileObjs = new List<List<GameObject>>();

	private tile cursor = new tile(gridSize/2,gridSize/2);
	private int direction;
 
	public Material testMaterial;

	public static float hexScale = 50;
    private Vector2 hexSize = new Vector2(hexScale,hexScale*Mathf.Sqrt(3)/2);
    public float gap = 0.0f;

	public int maxTiles;

	public GameObject finishPrefab;
	private GameObject finishTile;
	private GameObject cullTile;

	private int update;

	public struct tile{
		public int x;
		public int y;
	
		public tile(int a, int b){
			x = a;
			y = b;
		}
	}



	void Awake()
    {
		if (!finishTile) finishTile = Instantiate(finishPrefab);
		self = this;
		foreach (GameObject c in checkpoints){
			Destroy(c);
		}
		checkpoints.Clear();
		
		
		loadTiles();
		CreateGrid();
		
		for (int i = 0; i<5; i++){
			AutoTile();
		}
	
    }

	void loadTiles(){
		for (int i =0; i<5;i++){tileObjs.Add(new List<GameObject>());};
		foreach (GameObject obj in new List<GameObject>(Resources.LoadAll<GameObject>("Tiles"))){
			int i= obj.GetComponent<trackTile>().directionDelta+2;
			tileObjs[i].Add(obj);
		};
	}
	void FixedUpdate(){
		update++;
		finishTile.transform.position = grid[cursor.x,cursor.y];
		if (cullTile != carManager.leadTile) StartCoroutine(TileCulling());
		
	}

	IEnumerator TileCulling(){
		cullTile = carManager.leadTile;
		int i = tileHistory.IndexOf(cullTile) - maxTiles;
		if (i>0){
			while (i>0){
				i--;
				Destroy(tileHistory[0]);
				tileHistory.RemoveAt(0);
				yield return new WaitForSeconds(0);
			}
		}


	}

	void RemoveTile(int i){
			Destroy(tileHistory[0]);
			tileHistory.RemoveAt(0);
	}


	void CreateGrid(){
		for (int i = 0; i<gridSize; i++){
			for (int j = 0; j<gridSize; j++){
				grid[i,j] = new Vector3(i*hexSize.x,0,j*hexSize.y);
				if (j%2>0) grid[i,j].x += hexSize.x/2;
			}
		}
	}
	
	void AutoTile(){
		GameObject obj;
		int i =0;
		int d = 0;
		do{ 
			obj = GetRandomTile(Random.Range(-2,3));
			d = obj.GetComponent<trackTile>().directionDelta;
			i++;
			if (i>1000){
				Debug.Log("Endless Loop, deleting tile");
				tile current = cursor;
				MoveCursor(d);
				Destroy(placedTiles[cursor.x,cursor.y]);
				cursor = current;
				break;
			}
		}while (!validPath(d));
		CreateTile(obj);
		//tileHistory[tileHistory.Count-1].GetComponent<Renderer>().material = testMaterial;
	}

	public void newTile(int d){
		
		if (!validPath(d)){
			int j=0;
			do{
				tile current = cursor;
				AutoTile();
				if (cursor.x==current.x && cursor.y == current.y){
				Debug.Break();
				return;
			}
			}while(!validPath(d));
		}
		CreateTile(GetRandomTile(d));
	}


	void CreateTile(GameObject obj){
		
		GameObject o = Instantiate(obj,grid[cursor.x,cursor.y],Quaternion.Euler(0,120+direction*60,0), transform);
		placedTiles[cursor.x,cursor.y] = o;
		tileHistory.Add(o);
		o.name = "track " + cursor.x + ":" +cursor.y + " D:" + direction + " (" + obj.name +")";
		trackTile tile = o.GetComponent<trackTile>();
		direction = mod(direction+tile.directionDelta,6);
		tile.coordinates = cursor;
		checkpoint[] c = o.GetComponentsInChildren<checkpoint>();
		foreach (checkpoint p in c){
			addCheckpoint(p.gameObject,o);
		}
		
		MoveCursor(direction);

	}

	public void Clear(){
		foreach (GameObject o in tileHistory){
			Destroy(o);
		}
		grid = new Vector3[gridSize,gridSize];
		placedTiles = new GameObject[gridSize,gridSize];
		cursor = new tile(gridSize/2,gridSize/2);
		tileHistory.Clear();
		checkpoints.Clear();
		Awake();
		
	}

	GameObject GetRandomTile(int d){
		d+=2;
		return tileObjs[d][Random.Range(0,tileObjs[d].Count)];
	}

    
	void oldUpdate () {
		foreach( GameObject checkpoint in checkpoints){
			if (!checkpoint) checkpoints.Remove(checkpoint);
		}

		foreach( GameObject checkpoint in GameObject.FindGameObjectsWithTag("checkpoint")){
			if (checkpoints.IndexOf(checkpoint) < 0){
				
			}
		}
	}

	void addCheckpoint(GameObject checkpoint, GameObject parentTile){
		checkpointCounter++;
		if (checkpoints.Count>0) checkpoints[checkpoints.Count-1].transform.LookAt(checkpoint.transform);
		checkpoints.Add(checkpoint);
		checkpoint.GetComponent<checkpoint>().Init(checkpointCounter, parentTile);
		checkpoint.name = "Checkpoint " + checkpointCounter;
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

	bool validPath(int d){
		d=mod(direction+d,6);
		checkedTiles.Clear();
		checkedTiles.Add(cursor);
		tile current = cursor;
		MoveCursor(d);
		bool check = depthSearch(8);
		cursor = current;
		return check;
	}

	bool depthSearch(int depth){
		if (outOfBounds()) return false;
		checkedTiles.Add(cursor);
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
			return false;
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

		Gizmos.color=Color.red;
		foreach (tile t in checkedTiles){
			Gizmos.DrawWireSphere(grid[t.x,t.y],2);
		}
				Gizmos.color=Color.green;
		Gizmos.DrawWireSphere(grid[cursor.x,cursor.y],3);

        if (checkpoints.Count>2) {
			for (int i = 0; i<checkpoints.Count-1;i++){
				if (!checkpoints[i]) continue;
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

	public static int mod(int x, int m) {
		int r = x%m;
		return r<0 ? r+m : r;
	}

}
