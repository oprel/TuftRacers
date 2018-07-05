using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileLauncher : MonoBehaviour {

	public float origin;
	public GameObject prefab;
	public float cooldown = 2;
	public float cooldownTimer = 0;
	public float speed;

	void Update () {
		cooldownTimer -= Time.deltaTime;
		if (cooldownTimer<0){
			Fire();
		}
	}

	void Fire(){
		GameObject o = Instantiate(prefab, transform.position+origin * transform.forward, Quaternion.identity);
		Vector3 pulse = transform.forward * speed;
		o.GetComponent<Rigidbody>().velocity=pulse;
		cooldownTimer = cooldown;
		Destroy(o,6);

	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position+transform.forward*origin,.2f);
	}
}
