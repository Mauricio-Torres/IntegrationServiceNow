using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface IConectionService
    {
        Task<T> PostAsync<T>(string Token, string restUrl, object data) where T : new();
        Task<T> PutAsync<T>(string Token, string restUrl, object data) where T : new();
        Task<T> GetAsync<T>(string Token, string restUrl, string nameHeader, object data) where T : new();
        Task<T> GetAsync<T>(string Token, string restUrl) where T : new();
        Task PostAsync(string Token, string restUrl, object data);
    }
}
