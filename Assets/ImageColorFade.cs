using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorFade : MonoBehaviour {

	public AnimationCurve curve;
	public Gradient colors;
	public float duration = 2;
	public bool loop;
    private float timer = 0;
    private Image targetImage;
	
	// Use this for initialization
	void Start () {
		targetImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
        if (timer>duration){
            if (loop)timer =0;
            else this.enabled = false;
        } 
        float curvePoint = curve.Evaluate(timer/duration);
        targetImage.color = colors.Evaluate(curvePoint);
        
	}
}
