using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[DisallowMultipleComponent]
public class KinetixYield : MonoBehaviour
{
	private static volatile KinetixYield kinetixYieldInstance;

	private Queue<TaskCompletionSource<bool>> yield = new Queue<TaskCompletionSource<bool>>();

	[RuntimeInitializeOnLoadMethod]
	public static void CreateMe ()
	{
		if (kinetixYieldInstance != null)
			return;

		kinetixYieldInstance = new GameObject("KinetixYieldInstance").AddComponent<KinetixYield>();
	}

	public static Task Yield()
	{
		if (!kinetixYieldInstance)
			CreateMe();

		return kinetixYieldInstance.YieldInternal();
	}

	private Task YieldInternal()
	{
		TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
		yield.Enqueue(task);
		return task.Task;
	}

	private void Update()
	{
		if (yield.Count > 0)
		{
			yield.Dequeue().SetResult(true);
		}
	}

	private void OnDestroy()
	{
		if (kinetixYieldInstance == this)
			kinetixYieldInstance = null;

		Destroy(gameObject);
	}
}
