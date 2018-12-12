using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dynagon;

public class fieldGen : MonoBehaviour {

	public bool respawn;
	public int rings = 3;

	public float objSize = 2f;
	public float ringHeight =.1f;

	public int topVertices = 5;
	public int bottomVertices = 5;
	
	public Material material;
	List<Vector3> grid;
	// Use this for initialization
	void Start () {
		StartCoroutine(Generate());
	}

	private void Update() {
		if (respawn){
			respawn=false;
			foreach (Transform child in transform) {
				Destroy(child.gameObject);
			}
			StartCoroutine(Generate());
		}
	}

	private IEnumerator Generate(){
		grid = polarGrid();
		foreach (Vector3 center in grid){
			
			List<Vector3> verts = new List<Vector3>();
			Vector3 pos = center;
			verts.AddRange(randomVerts(pos, topVertices));
			pos.y-=ringHeight;
			verts.AddRange(randomVerts(pos, 4));
			pos.y=transform.position.y;
			verts.AddRange(randomVerts(pos, bottomVertices));
			GameObject o= Factory.Create("fieldsection",verts).gameObject;
			
			o.transform.parent = transform;
			//o.AddComponent<Rigidbody>();
			o.GetComponent<MeshRenderer>().material=material;
			Mesh mesh = o.GetComponent<MeshFilter>().mesh;
			Vector2[] uvs = mesh.uv;
			for (int i = 0; i < uvs.Length; i++)
				{
						uvs[i] = new Vector2(mesh.vertices[i].x-center.x, (mesh.vertices[i].y*mesh.normals[i].x + mesh.vertices[i].z*mesh.normals[i].y)-center.y); //magic
				}
			mesh.uv = uvs;
			yield return null;
		}
		foreach (Transform child in transform) {
			//child.gameObject.AddComponent<Rigidbody>();
		}
	}

	private List<Vector3> randomVerts(Vector3 center, int amount){
		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < amount; i++)
		{
			float offset = Random.Range(0,360);
			verts.Add(circlePoint(center,objSize,(float)i/amount * 360 + offset));
		}
		return verts;
	}
	


	private void OnDrawGizmosSelected() {
		List<Vector3> grid = polarGrid();
		foreach (Vector3 pos in grid){
			Gizmos.DrawWireSphere(pos,.1f);
		}
	}

	List<Vector3> polarGrid(){
		List<Vector3> grid = new List<Vector3>();
		//grid.Add(transform.position);
		for (int ring = 1; ring <= rings; ring++)
		{
			int amount = Mathf.FloorToInt(ring * Mathf.PI);
			//float offset = Random.Range(0,360);
			for (int i = 0; i < amount; i++)
			{
				float angle = (float)i/amount * 360;// + offset;
				Vector3 pos = circlePoint(transform.position, ring * objSize, angle);
				pos.y=ringHeight*(rings-ring+1) + transform.position.y;
				grid.Add(pos);
			}
			
		}
		return grid;
	}

	Vector3 circlePoint(Vector3 center, float radius, float angle){
		Vector3 pos = center;
		pos.x += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
		pos.z += radius * Mathf.Cos(angle * Mathf.Deg2Rad);
		return pos;
	}
}
