using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleProceduralTerrainProject;

[ExecuteInEditMode]
public class genManager : MonoBehaviour {

	public bool Regen;
	public int seed;
	[Header("genObjects")]
	public EditorTreePlacer trees;
	public TerrainGeneratorNew terrain;
	// Use this for initialization

	private void Update(){
		if (Regen){
			Regen=false;
			Reset();
		}
	}

	public void Reset(){
		terrain.m_seed = seed;
		terrain.reset = true;
		trees.Regen = true;
	}
}
