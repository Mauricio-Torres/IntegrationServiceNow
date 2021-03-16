using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Factories
{
    internal static class ContextInstance
    {
        private static readonly object singletonLock = new object();
        private static volatile FactoryInstance instance;
        public static FactoryInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (singletonLock)
                    {
                        if (instance == null)
                        {
                            instance = new FactoryInstance();
                        }
                    }
                }

                return instance;
            }
        }
    }

    internal class FactoryInstance
    {
        public readonly IUnityContainer Container;

        public FactoryInstance() 
        {
            Container = new UnityContainer();
            // Container.AddNewExtension<Factory>();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
