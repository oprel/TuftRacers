using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class postprocessingCamera : MonoBehaviour {

	public PostProcessVolume volume;
	public float depthOffset;
	private CameraController controller;
	private DepthOfField depth;

	void Awake(){
		controller = GetComponent<CameraController>();
	}
	void Start () {
		GetComponent<PostProcessLayer>().enabled=true;
		depth = ScriptableObject.CreateInstance<DepthOfField>();
		depth = (DepthOfField)volume.profile.settings[0];
	}
	
	// Update is called once per frame
	void Update () {
		float d = transform.position.y/(Mathf.Cos(transform.eulerAngles.x)*depthOffset);
		depth.focusDistance.value = d;
	}
}
