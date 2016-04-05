//TODO
//1. Calculate planes from normals for each fragment point
//2. Find plane intersections with other planes and the object wall
//3. generate a list of points to use as vertexes from the plane intersections
//4. create a mesh in the newly instantiated game objects for the fragments from the fracture
//5. DONEZO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
public class Cube : MonoBehaviour {

	public float width;
	public float height;
	public float length;
	public float fractureThickness;

	public int pointCount;

	public GameObject fragmentPrefab;

	List<Fragment> fragments;

	void Start() {
		Mesh _mesh = CreateMesh();
		GetComponent<MeshFilter>().mesh = _mesh;
		MeshCollider meshCollider = GetComponent(typeof(MeshCollider)) as MeshCollider;
		meshCollider.sharedMesh = _mesh;
		fragments = new List<Fragment>();
	}

	void Update() {
		//press space to spawn a new fragmentation
		if (Input.GetKeyUp(KeyCode.Space)) {
			//clear out the fragments list
			fragments.Clear ();
			List<Vector3>points = VoronoiPoints();
			//for each point generated find the vectors to other points and contain them in the Fragment struct
			foreach (Vector3 point in points) {
				Fragment master;
				master.position = point;
				master.rays = VectorsToNeighbors(point, points);
				fragments.Add(master);
			}
			//instantiate a new gameobject for each fragment to represent the fracture
			foreach (Fragment fragment in fragments) {
				GameObject fragObj = Instantiate(fragmentPrefab, fragment.position, Quaternion.identity) as GameObject;
				List<PlaneLine> intersections = CalculateVertices(fragment);
				fragObj.SendMessage("SetRays", fragment.rays);
				fragObj.SendMessage("SetLinePoint", intersections);
			}
		}
	}

	//create the mesh
	Mesh CreateMesh() {
		Mesh mesh = new Mesh ();

		Vector3[] vertices = new Vector3[] {
			new Vector3(width, height, length),
			new Vector3(width, height, -length),
			new Vector3(width, -height, length),
			new Vector3(width, -height, -length),
			new Vector3(-width, height, length),
			new Vector3(-width, height, -length),
			new Vector3(-width, -height, length),
			new Vector3(-width, -height, -length)
		};
		int[] triangles = new int[] {
			0, 2, 1,
			3, 1, 2,
			1, 3, 5,
			5, 3, 7,
			3, 2, 7,
			7, 2, 6,
			4, 5, 6,
			6, 5, 7,
			2, 0, 6, 
			0, 4, 6,
			0, 1, 5, 
			4, 0, 5
		};
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		return mesh;
	}

	//generate a list of random points inside of the object
	List<Vector3> VoronoiPoints() {
		List<Vector3> points = new List<Vector3>();
		for (int i = 0; i < pointCount; i++) {
			Vector3 point = new Vector3 (Random.Range (-width, width), Random.Range (-height, height), Random.Range (-length, length));
			points.Add(point);
		}
		return points;
	}

	//return the vectors pointing to each other neighboring point
	List<Vector3> VectorsToNeighbors(Vector3 self, List<Vector3> points) {
		List<Vector3> vectorsTo = new List<Vector3>();
		foreach (Vector3 point in points) {
			if (point != self) {
				Vector3 vector = point - self;
				vectorsTo.Add (vector);
			}
		};
		return vectorsTo;
	}

	//calculate the planes with the neighbor vectors as the normal vectors and then return a list of vertices from the plane intersections
	List<PlaneLine> CalculateVertices(Fragment frag) {
		List<PlaneLine> intersections = PlaneIntersections(frag);
		List<Vector3> planes = WallCast(intersections);
		return intersections;
	}

	List<PlaneLine> PlaneIntersections(Fragment frag) {
		List<PlaneLine> intersectionVectors = new List<PlaneLine>();
		foreach (Vector3 ray in frag.rays) {
			foreach (Vector3 rey in frag.rays) {
				Vector3 rayMid = frag.position + ray * .5f;
				Vector3 reyMid = frag.position + rey * .5f;
				Vector3 rayNorm = ray.normalized;
				Vector3 reyNorm = rey.normalized;
				if (ray != rey) {
					Vector3 lineVec = Vector3.Cross(rayNorm, reyNorm);
					Vector3 lineDir = Vector3.Cross(reyNorm, lineVec);
					float denominator = Vector3.Dot(rayNorm, lineDir);
					Vector3 plane2plane = rayMid - reyMid;
					float t = Vector3.Dot(rayNorm, plane2plane) / denominator;
					Vector3 linePoint = reyMid + t * lineDir;
					PlaneLine line = new PlaneLine(linePoint, lineVec, lineDir);
					intersectionVectors.Add(line);
				}
			}
		}
		return intersectionVectors;
	}

	List<Vector3> WallCast(List<PlaneLine> intersectionVectors) {
		List<Vector3> hits = new List<Vector3>();
		foreach (PlaneLine line in intersectionVectors) {
			RaycastHit hit;
			if (Physics.Raycast(line._point, line._line, out hit)) {
				print(hit.point);
				hits.Add(hit.point);
			}
			if (Physics.Raycast(line._point, line._orth, out hit)) {
				print(hit.point);
				hits.Add(hit.point);
			}
		}
		return hits;
	}
		
	//contain info for each fragment of the fractured asset
	struct Fragment {
		public List<Vector3> rays;
		public Vector3 position;
	}
}
