using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models.Configuration;

namespace Aranda.Integration.ServiceNow.Services
{
    internal class BaseService
    {
        public readonly IConectionService ConnectionService;
        private readonly IConfigureRepository ConfigureService;
        public IntegrationConfiguration Configuration { set; get; }

        internal BaseService(IConectionService conectionService, IConfigureRepository configureService)
        {
            ConnectionService = conectionService;
            ConfigureService = configureService;
            Configuration = ConfigureService.GetConfiguration();
        }
    }
}
