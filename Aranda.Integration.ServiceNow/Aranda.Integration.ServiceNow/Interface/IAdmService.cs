using Aranda.Integration.ServiceNow.Models.CI;
using Aranda.Integration.ServiceNow.Models.ResponseAdmApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface IAdmService
    {
        Task<List<Device>> GetItemComputerCI(Dictionary<string, string> searchItems);
        
    }
}
