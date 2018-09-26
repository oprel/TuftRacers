﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorObjectPlacer : MonoBehaviour {
	[Header("How many?")]
	public int Amount;
	
	[Header("Settings")]
	public float outerRadius;
	public float innerRadius;
	public float rayHeight;
	public int batchSize;
	public bool faceCenter = true;
	
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
			pos.y=0;
		}while(Vector3.Distance(Vector3.zero,pos)<innerRadius);
		pos+=transform.position;
		pos.y=rayHeight;
		RaycastHit hit;
		if (Physics.Raycast(pos, -Vector3.up, out hit, 2*rayHeight))
        {
            GameObject obj = Instantiate(objPrefabs[Random.Range(0,objPrefabs.Length)],transform);
			obj.transform.position=hit.point;
			if (faceCenter){
				Vector3 target = new Vector3(0,obj.transform.position.y,0);
				obj.transform.LookAt(target);
			}else{
				obj.transform.rotation = Quaternion.Euler(0,Random.Range(0,360),0);
			}
			
			allPlaced.Add(obj);
        }
		

	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,outerRadius);
		Vector3 circ = transform.position;
		//circ.y=rayHeight;
		Gizmos.color=Color.red;
		Gizmos.DrawWireSphere(transform.position,innerRadius);
	}
}
