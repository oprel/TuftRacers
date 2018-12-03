using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphenopsid : MonoBehaviour {
	public bool respawn = false;
	public GameObject jointPrefab;
	public GameObject branchPrefab;
	public float heightOffset = 4;
	public float radiusOffset = 1;
	public int jointAmount = 10;
	public float bendAmount = 5;
	private List<GameObject> joints = new List<GameObject>();
	// Use this for initialization
	void Start () {
		StartCoroutine(spawnTree());
	}

	void spawnBranches(Transform parent, int amount){
		for (int i = 0; i < amount; i++)
		{
			float angle = (float)i/amount * 360 + Random.Range(-5,5);
			Quaternion rot = Quaternion.Euler(0,angle,0);
			Vector3 pos = rot * parent.forward * radiusOffset * parent.lossyScale.x;
			GameObject o =Instantiate(branchPrefab, parent);
			o.transform.position+=pos;
			o.transform.rotation *= rot;
			o.transform.localScale =new Vector3 (1,Random.value+.5f,Random.value+.5f);
		}
	}

	private IEnumerator spawnTree(){
		while(joints.Count>0){
			yield return null;
		}
		joints.Clear();
		Vector3 pos = transform.position;
		Quaternion rot = Quaternion.identity;
		float scale = 1;
		for (int i = 0; i < jointAmount; i++)
		{
			
			GameObject o = Instantiate (jointPrefab, transform);
			o.transform.position = pos;
			o.transform.rotation = rot;
			o.transform.localScale *= scale;
			scale = 1-(float)i/jointAmount;
			pos += rot * transform.up * heightOffset * scale;
			rot *= Quaternion.Euler(Random.Range(-bendAmount,bendAmount),Random.value*360, Random.Range(-bendAmount,bendAmount));

			spawnBranches(o.transform, (jointAmount-i)+Random.Range(0,5));
			joints.Add(o);
			yield return new WaitForSeconds(Random.value+.5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (respawn){
			respawn=false;
			foreach (Transform child in transform) {
				Destroy(child.gameObject);
			}
			spawnTree();
		}
	}

	private void OnCollisionEnter(Collision other) {
		StopAllCoroutines();
		foreach (GameObject joint in joints){
			StartCoroutine(disconnect(joint));
		}
		StartCoroutine(spawnTree());
	}

	private IEnumerator disconnect(GameObject o){
		o.AddComponent<Rigidbody>();
		yield return new WaitForSeconds(3+Random.value*6);
		joints.Remove(o);
		Destroy(o);
	}

}
