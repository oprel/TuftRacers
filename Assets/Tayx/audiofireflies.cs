using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiofireflies : MonoBehaviour {

	public GameObject flyPrefab;
	public Vector2 Dimensions = new Vector2(10,10);
	private AudioPeer audioPeer;
	private GameObject[] flies = new GameObject[64];
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 64; i++)
		{
			GameObject o = Instantiate(flyPrefab,transform);
			o.transform.position = new Vector3(i*Dimensions.x,0,0);
			flies[i]=o;
		}
		audioPeer = GetComponent<AudioPeer>();
		
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < 64; i++){
			Vector3 pos = flies[i].transform.position;
			float a = audioPeer._audioBand64[i]*Dimensions.y;
			if (a>pos.y){ pos.y=a;}
			else if(pos.y>0){pos.y-=.1f;}
			flies[i].transform.position = pos;
		}
	}
}
