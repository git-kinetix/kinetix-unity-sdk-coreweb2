using System.Linq;
using UnityEngine;

public class RaycastBox : ARaycastObject
{
	public Vector3 size;

	public override void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		DrawGizmo(transform.position, transform.rotation);
		Gizmos.color = Color.white;
	}

	public override void DrawGizmo(Vector3 startPosition, Quaternion rotation)
	{
		Vector3 a = new Vector3( 1,-1, 1), b = new Vector3( 1,-1,-1), c = new Vector3(-1,-1,-1), d = new Vector3(-1,-1, 1),
			    e = new Vector3( 1, 1, 1), f = new Vector3( 1, 1,-1), g = new Vector3(-1, 1,-1), h = new Vector3(-1, 1, 1);

		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(rotation * offset + startPosition, rotation, size/2) * matrix;

		Gizmos.DrawLine(a, b);
		Gizmos.DrawLine(b, c);
		Gizmos.DrawLine(c, d);
		Gizmos.DrawLine(d, a);

		Gizmos.DrawLine(e, f);
		Gizmos.DrawLine(f, g);
		Gizmos.DrawLine(g, h);
		Gizmos.DrawLine(h, e);
		
		Gizmos.DrawLine(a, e);
		Gizmos.DrawLine(b, f);
		Gizmos.DrawLine(c, g);
		Gizmos.DrawLine(d, h);
		
		Gizmos.matrix = matrix;
	}

	public override Collider[] Overlap(Vector3 startPosition, Quaternion rotation, int layerMask)
	{
		return Physics.OverlapBox(startPosition + rotation * offset, size/2, rotation, layerMask)
			.Where(WhereColliderFilter).ToArray();
	}

	public override RaycastHit[] Raycast(Vector3 startPosition, Quaternion rotation, Vector3 direction, int layerMask, float distance = RAYCAST_DISTANCE)
	{
		return Physics.BoxCastAll(startPosition + rotation * offset, size/2, direction, rotation, distance, layerMask);
	}

	public override Vector3 FarthestPointOnCollider(Vector3 startPosition, Quaternion rotation, Vector3 direction)
	{
		Vector3 right   = rotation * Vector3.right   * size.x/2,
				up      = rotation * Vector3.up      * size.y/2,
				forward = rotation * Vector3.forward * size.z/2;

		Vector3 center = startPosition + rotation * offset;

		Vector3 intersectFaceRight   = right   * Mathf.Sign(Vector3.Dot(right  , direction)),
				intersectFaceUp      = up      * Mathf.Sign(Vector3.Dot(up     , direction)),
				intersectFaceForward = forward * Mathf.Sign(Vector3.Dot(forward, direction));

		return center +
			intersectFaceRight	+
			intersectFaceUp		+
			intersectFaceForward;

	}
}
