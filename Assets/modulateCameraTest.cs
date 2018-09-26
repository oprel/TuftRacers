using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class modulateCameraTest : MonoBehaviour {

	private Camera camera;
	public Material material;
	private float strength;

	// Use this for initialization
	private void Awake() {
		camera = GetComponent<Camera>();
	}
	
    private void Update() {
		strength = (Mathf.PingPong(Time.time, 2)-1)/500;
	}
 
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

		material.SetFloat("_Strength",strength);
        Graphics.Blit(source, destination, material);
	}

}
