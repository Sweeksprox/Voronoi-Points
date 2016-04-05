using UnityEngine;
using System.Collections;

public class PlaneLine{

	public Vector3 _point;
	public Vector3 _line;
	public Vector3 _orth;

	public PlaneLine(Vector3 point, Vector3 line, Vector3 orth) {
		this._line = line;
		this._point = point;
		this._orth = orth;
	}
}
