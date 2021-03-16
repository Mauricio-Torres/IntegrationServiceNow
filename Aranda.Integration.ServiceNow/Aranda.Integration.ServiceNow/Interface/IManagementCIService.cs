using Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    public interface IManagementCIService
    {
        Task<List<ResponseCreateCIApi>> Create(Dictionary<string, string> getCI);
    }
}
