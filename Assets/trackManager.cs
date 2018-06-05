using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class trackManager : MonoBehaviour {


	public static List<GameObject> checkpoints = new List<GameObject>();
	private int updateTimer;
	private LineRenderer LineRenderer;
	private int checkpointCounter;

	
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

	void OnDrawGizmos() {
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
}
