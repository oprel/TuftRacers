using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackTile : MonoBehaviour {

	public trackManager.tile coordinates;
	[Range(-2,2)]
	public int directionDelta = 0;

}
