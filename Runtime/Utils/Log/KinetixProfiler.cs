#if !UNITY_EDITOR
#undef KINETIX_PROFILER
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UObject = UnityEngine.Object;

namespace Kinetix.Internal.Utils
{
    internal static class KinetixProfiler
	{
		internal static bool _isDirty { get; private set; }
		internal static void OnProfilerViewReloaded() => _isDirty = false;

		internal static bool _isWindowOpen = false;
		public static bool s_enableCustomProfiler=true;

		[Serializable]
		internal class MethodTime
		{
			private static long startupTime;

			[RuntimeInitializeOnLoadMethod]
			private static void InitializeOnLoad()
			{
				startupTime = DateTime.Now.Ticks;
			}


			public TimeSpan startTimeJitter;
			public TimeSpan endTimeJitter;
			public TimeSpan startTime;
			public readonly string methodName;
			public readonly CustomStopWatch stopwatch;
			public readonly int parent;
			public bool HasParent => parent != -1;

			public MethodTime(string methodName, int parent = -1)
			{
				this.methodName = methodName;
				this.parent = parent;
				stopwatch = new CustomStopWatch();
			}

			public MethodTime Start()
			{
				startTime = new TimeSpan(DateTime.Now.Ticks - startupTime);
				stopwatch.Start();
				return this;
			}

			public MethodTime Stop()
			{
				stopwatch.Stop();
				return this;
			}
		}

		volatile internal static Dictionary<int, int> _traceStackIndex = new Dictionary<int, int>();
		volatile internal static Dictionary<int, bool> _breakpoints = new Dictionary<int, bool>();
		volatile internal static Dictionary<int,string> _groupName = new Dictionary<int,string>();
		volatile internal static HoleList<List<MethodTime>> _traceByGroup = new HoleList<List<MethodTime>>();

		public static CustomStopWatch stopwatch = new CustomStopWatch();

#if KINETIX_PROFILER
		private static ulong groupI = 0;
#endif

		/// <summary>
		/// Synchronousely sample the cost of your code.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="targetObject"></param>
		public static void Start(string name, UObject targetObject = null)
		{
#if KINETIX_PROFILER
			Profiler.BeginSample(name, targetObject);
#endif
		}

		/// <summary>
		/// Don't forget to call <see cref="Start(int, string)"/> to avoid memory leak
		/// </summary>
		/// <returns></returns>
		public static int CreateGroup(string name = null)
		{
#if KINETIX_PROFILER
			if (!s_enableCustomProfiler)
				return 0;
			
			lock (_groupName)
			lock (_breakpoints)
			lock (_traceStackIndex)
			lock (_traceByGroup)
			{
				_traceByGroup.Add(new List<MethodTime>());
				int latestIndex = _traceByGroup.LatestIndex;
				_traceStackIndex[latestIndex] = -1;

				if (name != null)
					name = name + "#" + groupI++;
				else
					name = "unnamed_group#" + groupI++;
				
				_groupName[latestIndex] = name;
				_breakpoints[latestIndex] = true;
				
				return latestIndex;
			}
#else
			return 0;
#endif
		}

		/// <summary>
		/// Use this for async purposes. Dont forget to create a group via <see cref="CreateGroup"/>
		/// </summary>
		public static void Start(int group, string name)
		{
#if KINETIX_PROFILER
			if (!s_enableCustomProfiler)
				return;
			stopwatch.Start();
			MethodTime methodTime;
			lock (_traceStackIndex)
			lock (_traceByGroup)
			lock (_traceByGroup[group])
			{
				methodTime = new MethodTime(name, _traceStackIndex[group]);
				methodTime.Start();
				
				_traceByGroup[group].Add(methodTime);
				_traceStackIndex[group] = _traceByGroup[group].Count - 1;

			}
			stopwatch.Stop();
			methodTime.stopwatch.jitter += stopwatch.Elapsed.Ticks;
#endif
		}

		public static void End()
		{
#if KINETIX_PROFILER
			Profiler.EndSample();
#endif
		}

		/// <summary>
		/// Use this for async purposes.
		/// </summary>
		/// <seealso cref="Start"/>
		public static void End(int group)
		{
#if KINETIX_PROFILER
			if (!s_enableCustomProfiler)
				return;

			stopwatch.Start();
			MethodTime methodTime;
			lock (_traceStackIndex)
			lock (_traceByGroup)
			lock (_traceByGroup[group])
			{
				int v = _traceStackIndex[group];
				List<MethodTime> methodTimes = _traceByGroup[group];
				int count = methodTimes.Count;

				methodTime = methodTimes[v];
				methodTime.Stop();
				methodTimes[v] = methodTime;

				_traceStackIndex[group] = methodTime.parent;
			}
			stopwatch.Stop();
			methodTime.stopwatch.jitter += stopwatch.Elapsed.Ticks;
#endif
		}

		public static void EndGroup(int group)
		{
#if KINETIX_PROFILER
			if (!s_enableCustomProfiler)
				return;

			lock (_traceByGroup)
			lock (_traceStackIndex)
			lock (_groupName)
			lock (_breakpoints)
			{
				_isDirty = true;
				if (_isWindowOpen && _breakpoints[group]) return;
	
				_traceByGroup.RemoveAt(group);
				_traceStackIndex.Remove(group);
				_groupName.Remove(group);
			}
#endif
		}

		internal static Group ExportGroup(int id)
		{
			return new Group()
			{
				name = _groupName[id],
				trace = new List<MethodTime>(_traceByGroup[id]),
				traceStackIndex = _traceStackIndex[id],
			};
		}

		internal static void ImportGroup(Group group)
		{
			_traceByGroup.Add(group.trace);
			int latestIndex = _traceByGroup.LatestIndex;
			_traceStackIndex[latestIndex] = group.traceStackIndex;
			_breakpoints[latestIndex] = true;
			_groupName[latestIndex] = "Import#"+group.name;

			_isDirty = true;
		}

		[Serializable]
		internal class Group
		{
			public string name;
			public List<MethodTime> trace;
			public int traceStackIndex;
		}
	}
}
