using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Util to Gizmo.Draw stuff
/// </summary>
public class GizmoUtils : MonoBehaviour
{
	public bool enableGizmo = true;

	private double time;

	private readonly Queue<Action> actions = new Queue<Action>();
	private readonly List<Action> clearbleActions = new List<Action>();

	public void AddGizmo(Action action)
	{
#if UNITY_EDITOR
		if (time != Time.timeAsDouble)
		{
			actions.Clear();
			clearbleActions.Clear();
		}

		time = Time.timeAsDouble;
#endif
		actions.Enqueue(action);
	}

	public void Color(Color color) => AddGizmo(() => Gizmos.color = color);
	public void DrawWireSphere(Vector3 center, float radius) => AddGizmo(() => Gizmos.DrawWireSphere(center, radius));
	public void DrawSphere(Vector3 center, float radius) => AddGizmo(() => Gizmos.DrawSphere(center, radius));

	private void OnDrawGizmos()
	{
		if (!enableGizmo)
			return;

#if UNITY_EDITOR
		if (time != Time.timeAsDouble)
		{
			clearbleActions.Clear();
		}

		time = Time.timeAsDouble;
#endif

		int count = clearbleActions.Count;
		for (int i = 0; i < count; i++)
		{
			Action action = clearbleActions[i];
			action();
		}

		count = actions.Count;
		for (int i = 0; i < count; i++)
		{
			Action action = actions.Dequeue();
			action();
			clearbleActions.Add(action);
		}

	}
}
