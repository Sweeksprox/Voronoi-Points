//TODO
//create a mesh from a list of points generated from plane intersections

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FragScript : MonoBehaviour {

	public List<Vector3> rays;
	public List<PlaneLine> lines;

	void Start() {
		
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Destroy (gameObject);
		}
		foreach (Vector3 ray in rays) {
			Debug.DrawRay(transform.position, ray);
		}
		foreach (PlaneLine line in lines) {
			Debug.DrawRay(line._point, line._line * 50, Color.blue);
			Debug.DrawRay(line._point, -line._line * 50, Color.blue);
			Debug.DrawRay (line._point, line._orth * 50, Color.red);
		}
	}

	void SetRays(List<Vector3> vectors) {
		rays = vectors;
	}

	void SetLinePoint(List<PlaneLine> intersections) {
		lines = intersections;
	}
}
