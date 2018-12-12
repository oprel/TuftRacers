using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class grassTextureUpdater : MonoBehaviour {
	public Texture2D[] grass;
	// Use this for initialization
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < grass.Length; i++)
		{
			Shader.SetGlobalTexture("_Grass"+i, grass[i]);
		}
	}
}
