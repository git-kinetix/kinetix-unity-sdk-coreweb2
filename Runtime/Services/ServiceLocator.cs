using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.Internal
{
	public class ServiceLocator : IDisposable
	{
		private Dictionary<Type, IKinetixService> services;

		public ServiceLocator()
		{
			services = new Dictionary<Type, IKinetixService>();
		}

		public bool Register<TService>(TService service) where TService: IKinetixService
		{
			if (services.ContainsKey(typeof(TService))) return false;

			services.Add(typeof(TService), service);

			return true;
		}

		public TService Get<TService>() where TService: IKinetixService
		{
			IKinetixService returnValue;

			services.TryGetValue(typeof(TService), out returnValue);
			
			return (TService) returnValue;
		}
		public IKinetixService Get(Type type)
		{
			IKinetixService returnValue;

			services.TryGetValue(type, out returnValue);
			
			return returnValue;
		}

		public void Dispose()
		{
            foreach (IKinetixService service in services.Values)
            {
                if (service is IDisposable disp)
				{
					disp.Dispose();
				}
            }
        }
	}
}
