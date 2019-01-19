using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorTreePlacer : MonoBehaviour {
	[Header("How many?")]
	public int Amount;
	public bool Regen;
	
	[Header("Settings")]
	public float outerRadius;
	public float innerRadius;
	public float rayHeight;
	public int batchSize;
	public bool faceCenter = true;
	public Vector3 offset;
	public Vector3 rotation;
	public Material material;
	
	public GameObject[] objPrefabs;
	public List<GameObject> allPlaced = new List<GameObject>();

	void Update () {
		if (Regen){
			Regen = false;
			Reset();
		}
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
			pos = Random.insideUnitSphere * outerRadius * transform.lossyScale.x;
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
			obj.GetComponent<MeshRenderer>().material=material;
			obj.transform.position+=offset;
			obj.transform.rotation*=Quaternion.Euler(rotation);
			allPlaced.Add(obj);
        }
		

	}

	public void Reset(){
		foreach(GameObject obj in allPlaced){
			DestroyImmediate(obj);
		}
		allPlaced.Clear();
		for (int i = 0; i < Amount; i++)
		{
			PlaceObj();
		}
	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,outerRadius*transform.lossyScale.x);
		Vector3 circ = transform.position;
		//circ.y=rayHeight;
		Gizmos.color=Color.red;
		Gizmos.DrawWireSphere(transform.position,innerRadius*transform.lossyScale.x);
	}
}
