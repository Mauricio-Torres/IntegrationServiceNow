using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models.Input.ADM;
using Aranda.Integration.ServiceNow.Models.ResponseAdmApi;
using Aranda.Integration.ServiceNow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Services
{
    internal class AdmService : BaseService, IAdmService
    {
        private string Token { set; get; }
        public AdmService(IConectionService conectionService, IConfigureRepository configureService)
            : base(conectionService, configureService) 
        {
            Token = Configuration.TokenAdm;
        }
        public async Task<List<Device>> GetItemComputerCI(Dictionary<string, string> searchItems)
        {
            GetDeviceDto inputSearchDevice = new GetDeviceDto
            {
                search = searchItems.ContainsKey(Constants.Search) ? searchItems[Constants.Search] : "",
                orderField = "name",
                orderType = "ASC",
                pageIndex = 0,
                pageSize = 1
            };

            Device device = new Device
            {
                AgentProfile = "DEFAULT",
                AgentVersion = "9.9.2103.1001",
                CompleteIpAddress = "192.168.001.144",
                DaysSinceLastInventory = 1,
                Discovery = false,
                LastInventory = Convert.ToDateTime("2021-03-10T12:30:14.723-05:00"),
                Manufacturer = "Hewlett-Packard test 2",
                MemoryUsage = 47.183217421046628,
                ResponsibleUserEmail = "julieth.mancera@arandasoft.com",
                ResponsibleUserId = 371,
                UserName = "INTERSEQ.LOCAL.julieth.mancera",
                ResponsibleUserName = "",
                Status = "InventoryUpdated",
                Type = "Desktop",
                Virtualization = "HyperV2",
                Vpro = false,
                CreationDate = Convert.ToDateTime("2021-03-10T12:30:14.723-05:00"),
                Description = "Test desck ",
                DiskUsage = 40.2323,
                Domain = "INTERSEQ.LOCAL",
                Id = 20,
                Model = "asus",
                IpRegistred = "192.168.001.144",
                Name = "",
                OperatingSystem = "windows 10 home v9",
                OperatingSystemVersion = "v-9.0.1",
                Serial = "ANSJ10-JDAIJ-23222"
            };

            List<Device> devices =  //new List<Device>() { device };
                                    await GetDeviceDto(inputSearchDevice); 

            IEnumerable < Device> query = devices.Select(x => x);

            if (searchItems.ContainsKey(Constants.IdDevice) && !string.IsNullOrEmpty(searchItems[Constants.IdDevice]))
            {
                query = devices.Where(x => x.Id.ToString().Equals(searchItems[Constants.IdDevice], StringComparison.InvariantCultureIgnoreCase));
            }
            else if (searchItems.ContainsKey(Constants.TypeDevice) && !string.IsNullOrEmpty(searchItems[Constants.TypeDevice]))
            {
                query = devices.Where(x => x.Type.Equals(searchItems[Constants.TypeDevice], StringComparison.InvariantCultureIgnoreCase));
            }

            return query.ToList();
        }


        private async Task<List<Device>> GetDeviceDto(GetDeviceDto getDeviceDto)
        {
            string urlGetItemsAdm = Configuration.EndpointAdm + Constants.UrlListDevice;
            List<Device> devices = new List<Device>();

            ResponseDeviceApi answerDeviceApi= await ConnectionService.GetAsync<ResponseDeviceApi>(Token, urlGetItemsAdm, Constants.NameHeaderAdm, getDeviceDto);

            if (answerDeviceApi.TotalItems > 1)
            {
                getDeviceDto.pageSize = answerDeviceApi.TotalItems;

                ResponseDeviceApi answerDeviceApi1= await ConnectionService.GetAsync<ResponseDeviceApi>(Token, urlGetItemsAdm, Constants.NameHeaderAdm, getDeviceDto);
                devices.AddRange(answerDeviceApi1.Content);
            }
            else
            {
                devices.AddRange(answerDeviceApi.Content);
            }

            return devices;
        }
    }
}


