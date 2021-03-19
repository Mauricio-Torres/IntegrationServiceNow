using Aranda.Integration.ServiceNow.Extensions;
using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi;
using Aranda.Integration.ServiceNow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Services
{
    internal class CIService : BaseService, ICIService
    {
        private string Token { set; get; }
        public CIService(IConectionService conectionService, IConfigureRepository configureService) : 
            base(conectionService, configureService) 
        {
            Token = Constants.TypeAuthenticationBasic + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration.UserServiceNow}:{Configuration.PasswordServiceNow}"));
        }
        public async Task<string> CreateAttributeCI(string endPoint,  object obj, Dictionary<string, object> search, string selectItem = null)
        {
            ResponseGeneralSysId answerGetCompanyApi;
            ResponseGetAttributeApi item = null;

            string urlGetItem = endPoint.ConvertUrl(search);
            
            if (search.Count > 0 && urlGetItem.Contains("?"))
            {
                item = await ConnectionService.GetAsync<ResponseGetAttributeApi>(Token, urlGetItem);
            }

            if (item == null || item.result.Count == 0)
            {
                answerGetCompanyApi = await ConnectionService.PostAsync<ResponseGeneralSysId>(Token, endPoint, obj);
            }
            else
            {
                SysId sys_Id = new SysId();
                
                if (!string.IsNullOrWhiteSpace(selectItem))
                {
                    string sysId = "";

                    sysId = item.result.FirstOrDefault(x => x.name.Equals(selectItem, StringComparison.InvariantCultureIgnoreCase))?.sys_id;
                    if (string.IsNullOrWhiteSpace(sysId))
                    {
                        sysId = item.result.FirstOrDefault().sys_id;
                    }

                    sys_Id.sys_id = sysId;
                }
                else
                {
                    sys_Id.sys_id = item.result.FirstOrDefault().sys_id;
                }

                answerGetCompanyApi = new ResponseGeneralSysId
                {
                    result = sys_Id
                };
            }

            return answerGetCompanyApi.result.sys_id;
        }

        public async Task<ResponseCreateCIApi> CreateCI(string endPoint, object obj, Dictionary<string, object> search)
        {
            ResponseCreateCIApi answerGetCompanyApi;

            string urlGetItem = endPoint.ConvertUrl(search);

            ResponseGetAttributeApi item = await ConnectionService.GetAsync<ResponseGetAttributeApi>(Token, urlGetItem);

            if (item.result == null || item.result.Count == 0)
            {
                answerGetCompanyApi = await ConnectionService.PostAsync<ResponseCreateCIApi>(Token, endPoint, obj);
            }
            else
            {
                endPoint = endPoint + "/" + item.result[0].sys_id;
                answerGetCompanyApi = await ConnectionService.PutAsync<ResponseCreateCIApi>(Token, endPoint, obj);
            }

            return answerGetCompanyApi;
        }

        public async Task<bool> CreateRelationShip(string endPoint, object obj, Dictionary<string, object> search)
        {
            await ConnectionService.PostAsync(Token, endPoint, obj);
            return true;
        }
    }
}
