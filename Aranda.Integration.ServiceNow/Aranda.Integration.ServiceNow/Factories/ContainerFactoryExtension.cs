using Aranda.Integration.ServiceNow.Factories;
using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Repository;
using Aranda.Integration.ServiceNow.Services;
using Aranda.Integration.ServiceNow.Utils;
using Microsoft.Practices.Unity;

namespace Aranda.Integration.ServiceNow.Factories
{
    internal class ContainerFactoryExtension: UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IFactory, Factory>();

            Container.RegisterType<IConectionService, ConectionService>(TypeService.Conection.ToString());

            Container.RegisterType<IConfigureRepository, ConfigureRepository>(TypeService.ConfigureRepository.ToString());

            Container.RegisterType<ICIService, CIService>(TypeService.CI.ToString(), new InjectionConstructor(
                                        Container.Resolve<IConectionService>(TypeService.Conection.ToString()), 
                                        Container.Resolve<IConfigureRepository>(TypeService.ConfigureRepository.ToString())));
   
            Container.RegisterType<IAdmService, AdmService>(TypeService.Adm.ToString(), new InjectionConstructor(
                                        Container.Resolve<IConectionService>(TypeService.Conection.ToString()), 
                                        Container.Resolve<IConfigureRepository>(TypeService.ConfigureRepository.ToString())));
            
            Container.RegisterType<IManagementCIService, ManagementCIService>(TypeService.ManagementCI.ToString(), new InjectionConstructor(
                                        Container.Resolve<IConectionService>(TypeService.Conection.ToString()), 
                                        Container.Resolve<IConfigureRepository>(TypeService.ConfigureRepository.ToString()),
                                        Container.Resolve<IAdmService>(TypeService.Adm.ToString()),
                                        Container.Resolve<ICIService>(TypeService.CI.ToString())
                                        ));
        }
    }
}
