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

                Table table = Configuration.Table.FirstOrDefault(t => t.TypeDevice.Any(x => x == device.Type));

                List<FieldTable> fieldsWithReference = new List<FieldTable>();
                fieldsWithReference.AddReferences(table.Fields);
                
                Dictionary<string, object> tableResolved = new Dictionary<string, object>();

                foreach (var field in fieldsWithReference)
                {
                    Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasNotReferences);

                    string keyTableResolved = tableResolved.GetKeyProperty(tableReference.NameReference);

                    if (string.IsNullOrWhiteSpace(keyTableResolved))
                    {
                        if (tableHasNotReferences)
                        {
                            object dataResolved = await SolveReference(propertyDevice, tableReference, device);

                            if (field.NeedId)
                            {
                                string key = propertyDevice.GetKeyProperty(field.MapperTo);

                                if (key != null)
                                {
                                    propertyDevice[key] = dataResolved;
                                }
                                else
                                {
                                    propertyDevice.Add(field.MapperTo, dataResolved);
                                }
                            }

                            if (dataResolved == null)
                            {
                                string key = propertyDevice.GetKeyProperty(field.MapperTo);
                                propertyDevice.Remove(key);
                            }
                        }
                        else
                        {
                            tables = new Stack<Table>();
                            var tableWR = SolveTableWithReference(tableReference);

                            foreach (var item in tableWR)
                            {
                                var references = item.Fields.Where(reference => !string.IsNullOrWhiteSpace(reference.Reference)).ToList();

                                if (references.Count == 0)
                                {
                                    tableResolved.Add(item.NameReference, await SolveReference(propertyDevice, item, device));
                                }
                                else
                                {
                                    foreach (var r in references)
                                    {
                                        string key = propertyDevice.GetKeyProperty(r.MapperTo);

                                        if (key != null)
                                        {
                                            string value = tableResolved.GetKeyProperty(r.Reference);

                                            if (value != null)
                                            {
                                                propertyDevice[key] = tableResolved[value];
                                            }
                                        }
                                        else
                                        {
                                            string value = tableResolved.GetKeyProperty(r.Reference);
                                            if (value != null)
                                            {
                                                propertyDevice.Add(r.MapperTo, tableResolved[value]);
                                            }
                                        }
                                    }

                                    tableResolved.Add(item.NameReference, await SolveReference(propertyDevice, item, device));
                                }
                            }

                            string keyV = tableResolved.GetKeyProperty(field.Reference);

                            if (keyV != null)
                            {
                                string key = propertyDevice.GetKeyProperty(field.MapperTo);

                                if (key != null)
                                {
                                    propertyDevice[key] = tableResolved[keyV];
                                }
                                else
                                {
                                    propertyDevice.Add(field.MapperTo, tableResolved[keyV]);
                                }
                            }
                        }
                    }
                    else
                    {
                        string key = propertyDevice.GetKeyProperty(field.MapperTo);

                        if (key != null)
                        {
                            propertyDevice[key] = tableResolved[keyTableResolved];
                        }
                    }
                    
                }

                string urlCI = EndpointServiceNow + table.Name;
                ExpandoObject inputCreateCI = new ExpandoObject();
                inputCreateCI.AddProperty(propertyDevice, table.Fields, out bool validateRequiredFieldsCI);
               
                if (validateRequiredFieldsCI)
                {
                    Dictionary<string, object> keyValueSearch = new Dictionary<string, object>();
                    keyValueSearch.SetValueProperty(propertyDevice, table);

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


//private  void SetDataCI(Table table, Dictionary<string, object> propertyDeviceBase, Device device, out Dictionary<string, object> pairsData)
//{
//    pairsData = propertyDeviceBase;

//    List<FieldTable> fieldsWithReference = new List<FieldTable>();
//    fieldsWithReference.AddReferences(table.Fields);

//    foreach (var field in fieldsWithReference)
//    {
//        Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasNotReferences);

//        if (tableReference.HasReferencedData)
//        {
//           var listReferencedData =  CreateRelatedData(tableReference, device);
//        }

//        string urlAtributeCI = EndpointServiceNow + tableReference.Name;
//        string answerSys_Id = string.Empty;

//        if (tableHasNotReferences)
//        {
//            ExpandoObject inputCreateCIReference = new ExpandoObject();
//            inputCreateCIReference.AddProperty(propertyDeviceBase, tableReference.Fields, out bool validateRequiredFieldsAttribute);

//            if (validateRequiredFieldsAttribute)
//            {
//                Dictionary<string, object> valueSearch = new Dictionary<string, object>();
//                valueSearch.SetValueProperty(propertyDeviceBase, tableReference);

//                answerSys_Id = AttributeCIService.CreateAttributeCI(urlAtributeCI, inputCreateCIReference, valueSearch).Result;

//                if (field.NeedId)
//                {
//                    string key = propertyDeviceBase.GetKeyProperty(field.MapperTo);
//                    if (key != null)
//                    {
//                        pairsData[key] = answerSys_Id;
//                    }
//                }
//            }
//            else
//            {
//                string key = pairsData.GetKeyProperty(field.MapperTo);
//                pairsData.Remove(key);
//            } 
//        }
//        else
//        {
//           SetDataCI(tableReference, propertyDeviceBase, device, out pairsData);
//        }
//    }
//}



//if (table.NeedCategory && table.CategoryTableName != null && table.CategoryTableName.Count > 0)
//{
//    List<string> categoriesForDevice = new List<string>();
//    List<Table> listTableWithData = Configuration.Table.Where(t => table.CategoryTableName.Contains(t.NameReference)).ToList();

//    foreach (var tableWithData in listTableWithData)
//    {
//        List<RelationshipData> relatedData = Configuration.RelationshipData.Where(
//            x => x.TypeData.Equals(tableWithData.NameDataRelationship, StringComparison.InvariantCultureIgnoreCase) &&
//            x.TypeDevice.Any(d => d.Equals(device.Type, StringComparison.InvariantCultureIgnoreCase))).ToList();

//        List<object> listData = relatedData.Data();

//        foreach (var data in listData)
//        {
//            Dictionary<string, object> keyValuePairs = data.KeyValuePairs();
//            ExpandoObject inputCreateCategoryCI = new ExpandoObject();
//            inputCreateCategoryCI.AddProperty(keyValuePairs, tableWithData.Fields, out bool validateRequiredFieldsCategory);

//            string urlCategoryCI = EndpointServiceNow + tableWithData.Name;

//            if (validateRequiredFieldsCategory)
//            {
//                Dictionary<string, object> valueSearch = new Dictionary<string, object>();
//                valueSearch.SetValueProperty(keyValuePairs, tableWithData);
//                keyValuePairs.TryGetValue(tableWithData.SelectedItemSearch, out object value);
//                categoriesForDevice.Add(await AttributeCIService.CreateAttributeCI(urlCategoryCI, inputCreateCategoryCI, valueSearch, value.ToString()));
//            }
//        }
//    }
//}

//List<FieldTable> fieldsWithReference = new List<FieldTable>();
//fieldsWithReference.AddReferences(table.Fields);

//foreach (var field in fieldsWithReference)
//{
//    Table tableReference = Configuration.Table.GetTableReferenced(field.Reference, out bool tableHasNotReferences);

//    string urlAtributeCI = EndpointServiceNow + tableReference.Name;

//    if (tableHasNotReferences)
//    {
//        ExpandoObject inputCreateCIReference = new ExpandoObject();
//        inputCreateCIReference.AddProperty(propertyDevice, tableReference.Fields, out bool validateRequiredFieldsAttribute);

//        if (validateRequiredFieldsAttribute)
//        {
//            Dictionary<string, object> valueSearch = new Dictionary<string, object>();
//            valueSearch.SetValueProperty(propertyDevice, tableReference);

//            string answerSys_Id = await AttributeCIService.CreateAttributeCI(urlAtributeCI, inputCreateCIReference, valueSearch);

//            if (field.NeedId)
//            {
//                string key = propertyDevice.GetKeyProperty(field.MapperTo);
//                if (key != null)
//                {
//                    propertyDevice[key] = answerSys_Id;
//                }
//            }
//        }
//        else
//        {
//            string key = propertyDevice.GetKeyProperty(field.MapperTo);
//            propertyDevice.Remove(key);
//        }
//    }
//    else
//    {
//        SetDataCI(tableReference, propertyDevice, out Dictionary<string, object> pairsData);
//        propertyDevice = pairsData;
//    }
//}