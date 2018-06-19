using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager self;
	public Text centerDisplay;
	// Use this for initialization
	void Start () {
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void showText(string text){
		centerDisplay.enabled=true;
		centerDisplay.text = text;
		centerDisplay.color = Random.ColorHSV();
		StartCoroutine(HideText(centerDisplay,2));
	}

	public IEnumerator HideText(Text obj, float sec){
		yield return new WaitForSeconds(sec);
		obj.enabled = false;

	}

}
