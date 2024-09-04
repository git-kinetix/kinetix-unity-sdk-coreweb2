using System.Linq;
using UnityEngine;

/// <summary>
/// Sphere to do raycasting
/// </summary>
public class RaycastSphere : ARaycastObject
{
	public float radius;

	public override void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		DrawGizmo(transform.position, transform.rotation);
		Gizmos.color = Color.white;
	}

	public override void DrawGizmo(Vector3 startPosition, Quaternion rotation)
	{
		Gizmos.DrawWireSphere(rotation * offset + startPosition, radius);
	}

	public override Collider[] Overlap(Vector3 startPosition, Quaternion rotation, int layerMask)
	{
		return Physics.OverlapSphere(startPosition + rotation * offset, radius, layerMask)
			.Where(WhereColliderFilter).ToArray();
	}

	public override RaycastHit[] Raycast(Vector3 startPosition, Quaternion rotation, Vector3 direction, int layerMask, float distance = RAYCAST_DISTANCE)
	{
		return Physics.SphereCastAll(startPosition + (rotation * offset), radius, direction, distance, layerMask);
	}

	public override Vector3 FarthestPointOnCollider(Vector3 startPosition, Quaternion rotation, Vector3 direction)
	{
		Vector3 center = startPosition + rotation * offset;
		return center + (direction).normalized * radius;
	}
}
