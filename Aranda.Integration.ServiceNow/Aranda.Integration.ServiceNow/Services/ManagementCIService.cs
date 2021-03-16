using Aranda.Integration.ServiceNow.Extensions;
using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models;
using Aranda.Integration.ServiceNow.Models.Configuration;
using Aranda.Integration.ServiceNow.Models.ResponseAdmApi;
using Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi;
using Aranda.Integration.ServiceNow.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Services
{
    sealed class ManagementCIService : BaseService, IManagementCIService
    {
        private readonly ICIService AttributeCIService;
        private readonly IAdmService AdmService;
        public ManagementCIService(IConectionService conectionService,
                                   IConfigureRepository configureService,
                                   IAdmService admService,
                                   ICIService attributeCIService) :
        base(conectionService, configureService)
        {
            AttributeCIService = attributeCIService;
            AdmService = admService;
        }

        public async Task<List<ResponseCreateCIApi>> Create(Dictionary<string, string> getCI)
        {
            List<ResponseCreateCIApi> answer = new List<ResponseCreateCIApi>();

            var itemsAdm = await AdmService.GetItemComputerCI(getCI);

            foreach (var device in itemsAdm)
            {
                Dictionary<string, object> propertyDevice = GenericPropertyEntity<Device>.ModelProperty(device);

                Table table = Configuration.Table.FirstOrDefault(t => t.TypeHadware.Any(x => x == device.Type));
                List<Table> tablesWithoutReference = Configuration.Table.Where(t => t.Fields.Where(field => string.IsNullOrWhiteSpace(field.Reference)).ToList().Count == t.Fields.Count).ToList();

                List<FieldTable> fieldsWithReference = new List<FieldTable>();
                fieldsWithReference.AddReferences(table.Fields);

                foreach (var field in fieldsWithReference)
                {
                    Table tableReference = tablesWithoutReference.FirstOrDefault(t => t.NameReference.Equals(field.Reference, System.StringComparison.InvariantCultureIgnoreCase));

                    string urlAtributeCI = Configuration.EndpointServiceNow + tableReference.Name;

                    ExpandoObject inputCreateCIReference = new ExpandoObject();
                    inputCreateCIReference.AddProperty(propertyDevice, tableReference.Fields);

                    bool validateRequiredFields_Attribute = inputCreateCIReference.ValidateRequiredFields(tableReference.Fields);

                    if (validateRequiredFields_Attribute)
                    {
                        Dictionary<string, object> valueSearch = new Dictionary<string, object>();
                        valueSearch.SetValueProperty(propertyDevice, tableReference);

                        string answerSys_Id = await AttributeCIService.CreateAttributeCI(urlAtributeCI, inputCreateCIReference, valueSearch);

                        if (field.NeedId)
                        {
                            string key = propertyDevice.GetKeyProperty(field.MapperTo);
                            if (key != null)
                            {
                                propertyDevice[key] = answerSys_Id;
                            }
                        }
                    }
                    else
                    {
                        string key = propertyDevice.GetKeyProperty(field.MapperTo);
                        propertyDevice.Remove(key);
                    }
                }

                string urlCI = Configuration.EndpointServiceNow + table.Name;
                ExpandoObject inputCreateCI = new ExpandoObject();
                inputCreateCI.AddProperty(propertyDevice, table.Fields);

                bool validateRequiredFields_CI = inputCreateCI.ValidateRequiredFields(table.Fields);
               
                if (validateRequiredFields_CI)
                {
                    Dictionary<string, object> keyValueSearch = new Dictionary<string, object>();
                    keyValueSearch.SetValueProperty(propertyDevice, table);

                    answer.Add(await AttributeCIService.CreateCI(urlCI, inputCreateCI, keyValueSearch));
                }

                if (Configuration.Relationship.ListRelationship.Count > 0)
                {
                    string urlRelation = Configuration.EndpointServiceNow + Configuration.Relationship.Table;

                    Table Relationship = Configuration.Table.FirstOrDefault(t => t.Name.Equals(Configuration.Relationship.Table, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var relation in Configuration.Relationship.ListRelationship)
                    {
                        ExpandoObject atributeRelationship = new ExpandoObject();
                        atributeRelationship.AddProperty(propertyDevice, Relationship.Fields);

                        Dictionary<string, object> valueSearchRelationship = GenericPropertyEntity<InputRelationShip>.ModelProperty(relation);

                        await AttributeCIService.CreateRelationShip(urlRelation, atributeRelationship, valueSearchRelationship);
                    }
                }
            }

            return answer;
        }
    }
}
