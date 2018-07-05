using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleObjects : MonoBehaviour {

	public GameObject[] objs;

	public void toggleAll(){
		foreach (GameObject o in objs){
			o.SetActive(!o.activeInHierarchy);
		}
	}


}
