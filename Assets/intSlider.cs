using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class intSlider : MonoBehaviour {
	public Slider slider;
	public string label;
	private Text text;
	public int i;
	
	void Awake(){
		text = GetComponent<Text>();
		if (label.Length<1) label =text.text;
	}

	void Update () {
	 	i = Mathf.RoundToInt(slider.value);
		text.text = label + i;
		
	}
}
