using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushTest : MonoBehaviour {

	private MeshFilter filter;
	private Mesh mesh;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmos() {
		filter = GetComponent<MeshFilter>();
		mesh = filter.sharedMesh;
		foreach(Vector3 pos in mesh.vertices){
			Gizmos.DrawWireSphere(transform.position+pos,.1f);
		}
		Gizmos.color = Color.yellow;
			foreach(Vector3 pos in mesh.normals){
			Gizmos.DrawWireSphere(transform.position+pos,.1f);
		}
	

		/*for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int[] vertices = mesh.GetTriangles(i);
			foreach (int v in vertices ){
				float a = mesh.triangles[v];
			}
		}*/
	}
}
