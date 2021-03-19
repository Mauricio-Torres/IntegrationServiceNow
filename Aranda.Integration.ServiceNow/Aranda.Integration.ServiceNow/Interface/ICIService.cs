using Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface ICIService
    {
        Task<string> CreateAttributeCI(string endPoint,  object obj, Dictionary<string, object> search, string selectItem = null);
        Task<ResponseCreateCIApi> CreateCI(string endPoint, object obj, Dictionary<string, object> search);
        Task<bool> CreateRelationShip(string endPoint, object obj, Dictionary<string, object> search);

    }
}
