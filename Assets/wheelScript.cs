using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelScript : MonoBehaviour {

	public carController parent;
	
	  void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "destroy"){
            parent.Reset();
        }
            
    }
}
