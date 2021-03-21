﻿using Aranda.Integration.ServiceNow.Extensions;
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
        private Stack<Table> tables;

        private string EndpointServiceNow {set; get;}
        public ManagementCIService(IConectionService conectionService,
                                   IConfigureRepository configureService,
                                   IAdmService admService,
                                   ICIService attributeCIService) :
        base(conectionService, configureService)
        {
            AttributeCIService = attributeCIService;
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
                
                Dictionary<string, object> tableResolved = new Dictionary<string, object>();

                foreach (var field in fieldsWithReference)
                {
                    Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasNotReferences);

                    if (!tableResolved.ContainsKey(tableReference.NameReference, out string keyTableResolved))
                    {
                        if (tableHasNotReferences)
                        {
                            object dataResolved = await SolveReference(propertyDevice, tableReference, device);
                           
                            bool findKeyPDevice= propertyDevice.ContainsKey(field.MapperTo, out string nameKeyPDevice);
                           
                            if (field.NeedId)
                            {

                                if (findKeyPDevice)
                                {
                                    propertyDevice[nameKeyPDevice] = dataResolved;
                                }
                                else
                                {
                                    propertyDevice.Add(field.MapperTo, dataResolved);
                                }
                            }

                            if (dataResolved == null)
                            {
                                propertyDevice.Remove(nameKeyPDevice);
                            }
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
                                    tableResolved.Add(table.NameReference, await SolveReference(propertyDevice, table, device));
                                }
                                else
                                {
                                    foreach (var r in references)
                                    {
                                        if (propertyDevice.ContainsKey(r.MapperTo, out string keySetValue))
                                        {
                                            if (tableResolved.ContainsKey(r.Reference, out string keyValueTResolved))
                                            {
                                                propertyDevice[keySetValue] = tableResolved[keyValueTResolved];
                                            }
                                        }
                                        else
                                        {
                                            if (tableResolved.ContainsKey(r.Reference, out string keyValueTResolved))
                                            {
                                                propertyDevice.Add(r.MapperTo, tableResolved[keyValueTResolved]);
                                            }
                                        }
                                    }

                                    tableResolved.Add(table.NameReference, await SolveReference(propertyDevice, table, device));
                                }
                            }

                            if (tableResolved.GetKeyProperty(field.Reference, out string keyTResolved, out object valueTableResolved ))
                            {
                                if (propertyDevice.ContainsKey(field.MapperTo, out string keyPDevice))
                                {
                                    propertyDevice[keyPDevice] = valueTableResolved;
                                }
                                else
                                {
                                    propertyDevice.Add(field.MapperTo, valueTableResolved);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (propertyDevice.ContainsKey(field.MapperTo, out string keySetParameter))
                        {
                            propertyDevice[keySetParameter] = tableResolved[keyTableResolved];
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

                    answer.Add(await AttributeCIService.CreateCI(urlCI, inputCreateCI, keyValueSearch));
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

                        await AttributeCIService.CreateRelationShip(urlRelation, atributeRelationship, valueSearchRelationship);
                    }
                }
            }

            return answer;
        }

        private async Task <List<string>> CreateRelatedData(Table table, Device device)
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
                    categoriesForDevice.Add(await AttributeCIService.CreateAttributeCI(url, inputCreateCategoryCI, valueSearch, value.ToString()));
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

                    answerSys_Id = await AttributeCIService.CreateAttributeCI(urlAtributeCI, inputCreateCIReference, valueSearch);
                }

                return answerSys_Id;
            }
        }
        private Stack<Table> SolveTableWithReference(Table table)
        {
            Table tableReference;

            List<FieldTable> fieldsWithReference = new List<FieldTable>();
            fieldsWithReference.AddReferences(table.Fields);

            if (!tables.Contains(table))
            {
                tables.Push(table);
            }

            foreach (var field in fieldsWithReference)
            {
                tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasReferences);
                
                if (!tables.Contains(tableReference))
                {
                    tables.Push(tableReference);
                }

                if (!tableHasReferences)
                {
                    SolveTableWithReference(tableReference);
                }
            }

            return tables;
        }
    }
}