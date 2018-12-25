using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class vinePlacer : MonoBehaviour {
	[Header("How many?")]
	public int Amount;
	
	[Header("Settings")]
	public float outerRadius;
	public float innerRadius;
	public float rayHeight;
	public int batchSize;
	public float spherecastRadius = 1;
	
	public GameObject[] objPrefabs;
	public List<GameObject> allPlaced = new List<GameObject>();

	void Update () {
		//batches
		if (Amount-batchSize>allPlaced.Count){
			for(int i =0;i<batchSize;i++){
				PlaceObj();
			}
		}else if(Amount < allPlaced.Count-batchSize){
			for(int i =0;i<batchSize;i++){
				DestroyImmediate(allPlaced[0]);
			allPlaced.RemoveAt(0);
			}
		}
		//single
		if (Amount>allPlaced.Count){
			PlaceObj();
		}else if (Amount < allPlaced.Count){
			DestroyImmediate(allPlaced[0]);
			allPlaced.RemoveAt(0);
		}
	}

	public void PlaceObj(){
		Vector3 pos;
		do{
			pos = transform.position + Random.insideUnitSphere * outerRadius;
		}while(Vector3.Distance(Vector3.zero,pos)<innerRadius);
		pos.y=transform.position.y;
		RaycastHit hit;
		//if (Physics.Raycast(pos, -Vector3.up, out hit, 2*rayHeight))
        Debug.DrawLine(pos, pos + transform.up * rayHeight);
        if (Physics.SphereCast(pos, spherecastRadius, transform.up, out hit, rayHeight))
        {
            Debug.Log("PLACED VINE");
            GameObject obj = Instantiate(objPrefabs[Random.Range(0,objPrefabs.Length)],transform);
			obj.transform.position=hit.point;
			obj.transform.GetChild(0).position = pos;
			allPlaced.Add(obj);
        }
		

	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,outerRadius);
		Gizmos.DrawLine(transform.position,transform.position + new Vector3(0,rayHeight,0));

		Gizmos.color=Color.red;
		Gizmos.DrawWireSphere(transform.position,innerRadius);
	}
}
