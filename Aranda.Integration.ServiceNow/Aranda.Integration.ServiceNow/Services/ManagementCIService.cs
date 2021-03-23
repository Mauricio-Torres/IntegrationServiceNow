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
        private readonly ICIService CIService;
        private readonly IAdmService AdmService;
        private Stack<Table> tables;

        private string EndpointServiceNow { set; get; }
        public ManagementCIService(IConectionService conectionService,
                                   IConfigureRepository configureService,
                                   IAdmService admService,
                                   ICIService ciService) :
        base(conectionService, configureService)
        {
            CIService = ciService;
            AdmService = admService;

            EndpointServiceNow = Configuration.EndpointServiceNow;
        }

        public async Task<List<ResponseCreateCIApi>> Create(Dictionary<string, string> getCI)
        {
            List<ResponseCreateCIApi> answer = new List<ResponseCreateCIApi>();

            List<Device> itemsAdm = await AdmService.GetItemComputerCI(getCI);

            foreach (var device in itemsAdm)
            {
                Dictionary<string, object> propertyDevice = GenericPropertyEntity<Device>.ModelProperty(device);

                Table tableDevice = Configuration.Table.FirstOrDefault(t => t.TypeDevice.Any(x => x == device.Type));

                List<FieldTable> fieldsWithReference = new List<FieldTable>();
                fieldsWithReference.AddReferences(tableDevice.Fields);

                Dictionary<string, object> listResolvedReferences = new Dictionary<string, object>();

                foreach (var field in fieldsWithReference)
                {
                    Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasNotReferences);

                    if (!listResolvedReferences.ContainsKey(tableReference.NameReference, out string keyTableResolved))
                    {
                        if (tableHasNotReferences)
                        {
                            object dataResolved = await SolveReference(propertyDevice, tableReference, device);
                            listResolvedReferences.Add(tableReference.NameReference, dataResolved);
                            AddValuesDeviceProperties(field, dataResolved, in propertyDevice);
                        }
                        else
                        {
                            tables = new Stack<Table>();
                            Stack<Table> listTableResolved = SolveTableWithReference(tableReference);

                            foreach (var table in listTableResolved)
                            {
                                List<FieldTable> references = table.Fields.Where(reference => !string.IsNullOrWhiteSpace(reference.Reference)).ToList();

                                if (references.Count == 0)
                                {
                                    listResolvedReferences.Add(table.NameReference, await SolveReference(propertyDevice, table, device));
                                }
                                else
                                {
                                    foreach (var r in references)
                                    {
                                        if (r.NeedId)
                                        {
                                            if (propertyDevice.ContainsKey(r.MapperTo, out string keySetValue))
                                            {
                                                if (listResolvedReferences.ContainsKey(r.Reference, out string keyValueTResolved))
                                                {
                                                    propertyDevice[keySetValue] = listResolvedReferences[keyValueTResolved];
                                                }
                                            }
                                            else
                                            {
                                                if (listResolvedReferences.ContainsKey(r.Reference, out string keyValueTResolved))
                                                {
                                                    propertyDevice.Add(r.MapperTo, listResolvedReferences[keyValueTResolved]);
                                                }
                                            }
                                        }
                                    }

                                    listResolvedReferences.Add(table.NameReference, await SolveReference(propertyDevice, table, device));
                                }
                            }

                            if (listResolvedReferences.ContainsKey(field.Reference, out string keyTResolved, out object dataResolved))
                            {
                                AddValuesDeviceProperties(field, dataResolved, in propertyDevice);
                            }
                        }
                    }
                    else
                    {
                        if (field.NeedId && propertyDevice.ContainsKey(field.MapperTo, out string keySetParameter))
                        {
                            propertyDevice[keySetParameter] = listResolvedReferences[keyTableResolved];
                        }

                    }
                }

                string urlCI = EndpointServiceNow + tableDevice.Name;
                ExpandoObject inputCreateCI = new ExpandoObject();
                inputCreateCI.AddProperty(propertyDevice, tableDevice.Fields, out bool validateRequiredFieldsCI);

                if (validateRequiredFieldsCI)
                {
                    Dictionary<string, object> keyValueSearch = new Dictionary<string, object>();
                    keyValueSearch.SetValueProperty(propertyDevice, tableDevice);

                    answer.Add(await CIService.CreateCI(urlCI, inputCreateCI, keyValueSearch));
                }

                if (Configuration.Relationship.ListRelationship.Count > 0)
                {
                    string urlRelation = EndpointServiceNow + Configuration.Relationship.Table;

                    Table Relationship = Configuration.Table.FirstOrDefault(t => t.Name.Equals(Configuration.Relationship.Table, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var relation in Configuration.Relationship.ListRelationship)
                    {
                        ExpandoObject atributeRelationship = new ExpandoObject();
                        atributeRelationship.AddProperty(propertyDevice, Relationship.Fields, out bool validateRequiredFieldsRelationship);

                        Dictionary<string, object> valueSearchRelationship = GenericPropertyEntity<InputRelationShip>.ModelProperty(relation);

                        await CIService.CreateRelationShip(urlRelation, atributeRelationship, valueSearchRelationship);
                    }
                }
            }

            return answer;
        }

        private async Task<List<string>> CreateRelatedData(Table table, Device device)
        {
            List<string> categoriesForDevice = new List<string>();

            List<DataRelationship> relatedData = Configuration.RelationshipData.Where(
                x => x.TypeData.Equals(table.NameDataRelationship, StringComparison.InvariantCultureIgnoreCase) &&
                x.TypeDevice.Any(d => d.Equals(device.Type, StringComparison.InvariantCultureIgnoreCase))).ToList();

            List<object> listData = relatedData.Data();

            foreach (var data in listData)
            {
                Dictionary<string, object> keyValuePairs = data.KeyValuePairs();

                ExpandoObject inputCreateCategoryCI = new ExpandoObject();
                inputCreateCategoryCI.AddProperty(keyValuePairs, table.Fields, out bool validateRequiredFieldsCategory);

                string url = EndpointServiceNow + table.Name;

                if (validateRequiredFieldsCategory)
                {
                    Dictionary<string, object> valueSearch = new Dictionary<string, object>();
                    valueSearch.SetValueProperty(keyValuePairs, table);
                    keyValuePairs.TryGetValue(table.SelectedItemSearch, out object value);
                    categoriesForDevice.Add(await CIService.CreateAttributeCI(url, inputCreateCategoryCI, valueSearch, value.ToString()));
                }
            }

            return categoriesForDevice;
        }

        private async Task<object> SolveReference(Dictionary<string, object> propertyDevice, Table tableReference, Device device)
        {
            if (tableReference.HasReferencedData)
            {
                return await CreateRelatedData(tableReference, device);
            }
            else
            {
                string urlAtributeCI = EndpointServiceNow + tableReference.Name;
                string answerSys_Id = string.Empty;

                ExpandoObject inputCreateCIReference = new ExpandoObject();
                inputCreateCIReference.AddProperty(propertyDevice, tableReference.Fields, out bool validateRequiredFieldsAttribute);

                if (validateRequiredFieldsAttribute)
                {
                    Dictionary<string, object> valueSearch = new Dictionary<string, object>();
                    valueSearch.SetValueProperty(propertyDevice, tableReference);

                    answerSys_Id = await CIService.CreateAttributeCI(urlAtributeCI, inputCreateCIReference, valueSearch);
                }

                return answerSys_Id;
            }
        }


        private void AddValuesDeviceProperties(FieldTable field, object dataResolved, in Dictionary<string, object> propertyDevice)
        {
            if (field.NeedId)
            {
                if (propertyDevice.ContainsKey(field.MapperTo, out string nameKeyPDevice))
                {
                    propertyDevice[nameKeyPDevice] = dataResolved;
                }
                else
                {
                    propertyDevice.Add(field.MapperTo, dataResolved);
                }
            }
        }

        private Stack<Table> SolveTableWithReference(Table table)
        {
            List<FieldTable> fieldsWithReference = new List<FieldTable>();
            fieldsWithReference.AddReferences(table.Fields);

            if (!tables.Contains(table))
            {
                tables.Push(table);
            }

            if (!fieldsWithReference.Any(f => f.Reference.Equals(table.NameReference)))
            {
                foreach (var field in fieldsWithReference)
                {
                    Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasReferences);

                    if (!tables.Contains(tableReference))
                    {
                        tables.Push(tableReference);
                    }

                    if (!tableHasReferences)
                    {
                        SolveTableWithReference(tableReference);
                    }
                }
            }

            return tables;
        }
    }
}