using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Repository;
using Aranda.Integration.ServiceNow.Services;
using Aranda.Integration.ServiceNow.Utils;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Factories
{
    internal class Factory: IFactory
    {
        public IUnityContainer Container { private set; get; }


        public Factory(IUnityContainer container)
        {
            Container = container;
        }


        public IManagementCIService Create(TypeService typeService)        
        {
            return Container.Resolve<IManagementCIService>(typeService.ToString());
            //switch (typeService)
            //{
            //    case TypeService.ManagementCI:
            //        return Container.Resolve<IManagementCIService>(typeService.ToString());
            //    case TypeService.Conection:
            //        return Container.Resolve<IConectionService>(typeService.ToString());
            //    case TypeService.CI:
            //        return Container.Resolve<ICIService>(typeService.ToString());
            //    case TypeService.Adm:
            //        return Container.Resolve<IAdmService>(typeService.ToString());
            //    case TypeService.ConfigureRepository:
            //        return Container.Resolve<IConfigureRepository>(typeService.ToString());
            //    default:
            //        return Container.Resolve<IManagementCIService>();
            //}
        }

    }
}
