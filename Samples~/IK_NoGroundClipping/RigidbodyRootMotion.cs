using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyRootMotion : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private Transform source;
	private Vector3 previousPos = Vector3.zero;

	private void LateUpdate()
	{
		Vector3 acceleration = source.localPosition - previousPos;
		acceleration.y = 0;
		rb.position += acceleration;

		previousPos = source.localPosition;

		source.localPosition = new Vector3(0, source.localPosition.y, 0);
		source.localRotation = Quaternion.identity;
	}
}
