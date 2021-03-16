using Aranda.Integration.ServiceNow.Factories;
using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi;
using Aranda.Integration.ServiceNow.Utils;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Client
{
    public class IntegrationServiceNow
    {
        private readonly IManagementCIService ManagmentService;
        private readonly IUnityContainer Container;

        public IntegrationServiceNow()
        {
            Container = new UnityContainer();
            Container.AddNewExtension<ContainerFactoryExtension>();
            IFactory factory = Container.Resolve<IFactory>();
            ManagmentService = factory.Create(TypeService.ManagementCI);
        }

        public async Task<List<ResponseCreateCIApi>> CreateCI(Dictionary<string, string> parameters)
        {
            List<ResponseCreateCIApi> actionResult;
            try
            {
                actionResult = await ManagmentService.Create(parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return actionResult;
        }
    }
}
