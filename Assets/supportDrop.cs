using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class supportDrop : MonoBehaviour {
public float force;

	public void Drop(Vector3 pos){
        transform.SetParent(null);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForceAtPosition(force * Random.insideUnitSphere,pos);
        transitionManager.fadeOut(gameObject,100);
    }
}
