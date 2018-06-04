using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager : MonoBehaviour {

	public static List<carController> cars = new List<carController>();
	public static Vector3 averagePos;
	public GameObject circle;

	// Use this for initialization
	private void FindAveragePosition()
		{
			averagePos = new Vector3();
			int numTargets = 0;

			for (int i = 0; i < cars.Count; i++)
			{
				if (!cars[i].gameObject.activeSelf)
					continue;

				averagePos += cars[i].transform.position;
				numTargets++;
			}

			if (numTargets > 0)
				averagePos /= numTargets;
		}
	
	// Update is called once per frame
	void FixedUpdate () {
		FindAveragePosition();
		circle.transform.position = averagePos;
		
	}

	void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(averagePos, 2f);
    }

	

}
