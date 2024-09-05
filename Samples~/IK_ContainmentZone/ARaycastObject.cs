using System;
using System.Linq;
using UnityEngine;

public abstract class ARaycastObject : MonoBehaviour
{
	/// <summary>
	/// Max default raycast distance
	/// </summary>
	protected const float RAYCAST_DISTANCE = 1f;

	/// <summary>
	/// Offset between the global transform position and the center of the RaycastObject
	/// </summary>
	public Vector3 offset;
	public GameObject[] ignoreColliders;

	/// <summary>
	/// Check if the object overlaps with any collider
	/// </summary>
	/// <param name="startPosition">Global position of the transform</param>
	/// <param name="rotation">Global rotation of the transform</param>
	/// <param name="layerMask">Layers on which to test</param>
	/// <returns>List of colliders that have overlapped</returns>
	abstract public Collider[] Overlap(Vector3 startPosition, Quaternion rotation, int layerMask);
	/// <summary>
	/// Raycast the object along a line to check for hits with a collider
	/// </summary>
	/// <param name="startPosition">Global position of the transform</param>
	/// <param name="rotation">Global rotation of the transform</param>
	/// <param name="direction">Direction of the raycast</param>
	/// <param name="layerMask">Layers on which to test</param>
	/// <param name="hitInfo">Infos of the hit</param>
	/// <param name="distance">Maximum distance of the raycast</param>
	/// <returns>True if there is a hit</returns>
	public bool Raycast(Vector3 startPosition, Quaternion rotation, Vector3 direction, int layerMask, out RaycastHit hitInfo, float distance = RAYCAST_DISTANCE, Collider colliderToFind = null)
	{
		//Get the hit that is the collider to find (if not null)
		//Or get the hit that isn't in the ignored list
		RaycastHit[] hits = Raycast(startPosition, rotation, direction, layerMask, distance)
			.Where(colliderToFind == null ? WhereColliderFilter : WhereHasSpecificColliderRaycast(colliderToFind))
			.ToArray();

		if (hits == null || hits.Length == 0) //If there is no corrisponding hit
		{
			hitInfo = default;
			return false;
		}

		//Get the farthest hit
		hitInfo = hits.First();
		float max = hitInfo.distance;
		for (int i = hits.Length - 1; i >= 1; i--)
		{
			if (hits[i].distance > max)
				hitInfo = hits[i];
		}

		return true;

	}

	abstract public RaycastHit[] Raycast(Vector3 startPosition, Quaternion rotation, Vector3 direction, int layerMask, float distance = RAYCAST_DISTANCE);
	/// <summary>
	/// Get the farthest point (in global context) along the requested direction
	/// </summary>
	/// <param name="startPosition">Global position of the transform</param>
	/// <param name="rotation">Global rotation of the transform</param>
	/// <param name="direction">Direction to request</param>
	/// <returns>The farthest point on collider in the requested direction</returns>
	abstract public Vector3 FarthestPointOnCollider(Vector3 startPosition, Quaternion rotation, Vector3 direction);

	abstract public void OnDrawGizmosSelected();
	/// <summary>
	/// Draw gizmos (in a OnDrawGizmo method)
	/// </summary>
	/// <param name="startPosition">Global position of the transform</param>
	/// <param name="rotation">Global rotation of the transform</param>
	public abstract void DrawGizmo(Vector3 startPosition, Quaternion rotation);



	private bool DistanceSecurity(RaycastHit hit)
	{
		return hit.distance > Mathf.Epsilon;
	}

	protected bool WhereColliderFilter(RaycastHit raycastHit) => DistanceSecurity(raycastHit) && !ignoreColliders.Contains(raycastHit.collider.gameObject);
	protected bool WhereColliderFilter(Collider collider) => !ignoreColliders.Contains(collider.gameObject);

	protected Func<Collider, bool> WhereHasSpecificCollider(Collider colliderToFind)
	{
		return Where;
		bool Where(Collider other)
		{
			return other == colliderToFind;
		}
	}
	protected Func<RaycastHit, bool> WhereHasSpecificColliderRaycast(Collider colliderToFind)
	{
		return Where;
		bool Where(RaycastHit raycastHit)
		{
			return DistanceSecurity(raycastHit) && raycastHit.collider == colliderToFind;
		}
	}
}
