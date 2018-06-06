using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class trackManager : MonoBehaviour {


	public static List<GameObject> checkpoints = new List<GameObject>();

	static int gridSize = 20;
	public static Vector3[,] grid = new Vector3[gridSize,gridSize];
	public static GameObject[,] placedTiles = new GameObject[gridSize,gridSize];
	private int updateTimer;
	private int checkpointCounter;

	public GameObject hexPrefab;

	private tile cur = new tile(10,10);
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
	
    void Start()
    {

		
		CreateGrid();
		direction=-1;
		for (int i = 0; i<20; i++){
			direction = mod((direction + Random.Range(-1,1)),6);
			//direction = mod(direction+ 1,6);
			NextTile();
			Debug.DrawLine(grid[cur.x,cur.y], Vector3.up+ grid[cur.x,cur.y] + Quaternion.Euler(0,30+direction*60,0) * Vector3.forward * 20,Color.red, 200);
			
			GameObject o = Instantiate(hexPrefab,grid[cur.x,cur.y],Quaternion.identity, transform);
			placedTiles[cur.x,cur.y]  = o;
			//checkpoints.Add(o);
			o.name = i + "|| " +cur.x + " : " + cur.y + " HEX D:" + direction;

			
		}
		/* 
		for (int i = 0; i<gridSize; i++){
			for (int j = 0; j<gridSize; j++){
				Instantiate(hexPrefab,grid[i,j],Quaternion.identity, transform);
			}
		}*/
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

	void NextTile(){
		int d = direction;
		switch (d){
			case 0:
			cur.y++;
			if (cur.y%2==0)cur.x++;
			return;
			case 1:
			cur.x++;
			return;
			case 2:
			cur.y--;
			if (cur.y%2==0)cur.x++;
			return;
			case 3:
			cur.y--;
			if (cur.y%2==1)cur.x--;
			return;
			case 4:
			cur.x--;
			return;
			case 5:
			cur.y++;
			if (cur.y%2==1)cur.x--;
			return;



		}
		/*
		
		if (d>2){ 
			d-=3;
			cur.x++;}
		else if (d==1){
			cur.x--;
		}
		if (d==0) cur.y++; 
		if (d==2) cur.y--;
		*/
	}

	void OnDrawGizmos() {
		CreateGrid();
		for (int i = 0; i<gridSize-2; i++){
			for (int j = 0; j<gridSize-2; j++){
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
