using Aranda.Integration.ServiceNow.Models.ResponseAdmApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface IAdmService
    {
        Task<List<Device>> GetItemComputerCI(Dictionary<string, string> searchItems); 
    }
}
