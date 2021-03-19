﻿using Aranda.Integration.ServiceNow.Models.Configuration;
using Aranda.Integration.ServiceNow.Services;
using Aranda.Integration.ServiceNow.Utils;
using FastMember;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Extensions
{
    internal static class ObjectExtension
    {
        private static object AddProperty(this object expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;

            if (!string.IsNullOrEmpty(propertyValue.ToString()))
            {
                if (expandoDict.ContainsKey(propertyName))
                {
                    expandoDict[propertyName] = propertyValue;
                }
                else
                {
                    expandoDict.Add(propertyName, propertyValue);
                }
            }

            return expandoDict;
        }

        public static object AddProperty(this object expando, Dictionary<string, object> propertiesValue)
        {
            var expandoDict = expando as IDictionary<string, object>;

            foreach (var item in propertiesValue)
            {
                if (item.Value != null || string.IsNullOrEmpty(item.Value.ToString()))
                {
                    if (expandoDict.ContainsKey(item.Key))
                    {
                        expandoDict[item.Key] = item.Value;
                    }
                    else
                    {
                        expandoDict.Add(item.Key, item.Value);
                    }
                }
            }

            return expandoDict;
        }

        private static bool ValidateRequiredFields(this ExpandoObject obj, List<FieldTable> fieldTables)
        {
            List<FieldTable> fieldsRequired = fieldTables.Where(x => x.Required).ToList();

            bool control = fieldsRequired.Count == 0;

            foreach (var fieldRequired in fieldsRequired)
            {
                var nameProperty = obj.GetMemberNames().ToList();

                control = nameProperty.Any(x => x == fieldRequired.Name);
                if (!control)
                {
                    break;
                }
            }

            return control;
        }

        public static void AddProperty(this ExpandoObject expando, Dictionary<string, object> propertiesValue, List<FieldTable> fieldTables, out bool validationFields)
        {
            var expandoDict = expando as IDictionary<string, object>;

            List<FieldTable> fieldsMapperCI = fieldTables.Where(f => f.Mapper).ToList();
            List<FieldTable> fieldsNoMapperCI = fieldTables.Where(f => !f.Mapper).ToList();

            foreach (var item in fieldsMapperCI)
            { 
                if (!string.IsNullOrWhiteSpace(item.MapperTo))
                {
                    var property = propertiesValue.FirstOrDefault(x => x.Key.Equals(item.MapperTo, StringComparison.InvariantCultureIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(property.Key) && property.Value != null && !string.IsNullOrEmpty(property.Value.ToString()))
                    {
                        if (expandoDict.ContainsKey(item.Name))
                        {
                            expandoDict[item.Name] = property.Value;
                        }
                        else
                        {
                            switch (item.Type)
                            {
                                case "int":
                                    if (int.TryParse(property.Value.ToString(), out int resInt))
                                    {
                                        expandoDict.Add(item.Name, resInt);
                                    }
                                    break;
                                case "long":
                                    if (long.TryParse(property.Value.ToString(), out long resLong))
                                    {
                                        expandoDict.Add(item.Name, resLong);
                                    }
                                    break;
                                case "decimal":
                                    if (float.TryParse(property.Value.ToString(), out float resFloat))
                                    {
                                        expandoDict.Add(item.Name, resFloat);
                                    }
                                    break;
                                case "boolean":
                                    if (bool.TryParse(property.Value.ToString(), out bool resBoolean))
                                    {
                                        expandoDict.Add(item.Name, resBoolean);
                                    }
                                    break;
                                case "date":
                                    if (DateTime.TryParse(property.Value.ToString(), out DateTime resDate))
                                    {
                                        expandoDict.Add(item.Name, resDate);
                                    }
                                    break;
                                case "array":
                                    string[] arr = ((IEnumerable)property.Value).Cast<object>()
                                                     .Select(x => x.ToString())
                                                     .ToArray();
                                    StringBuilder sbuilder = new StringBuilder();
                                    foreach (string value in arr)
                                    {
                                        sbuilder.Append(value);
                                        sbuilder.Append(',');
                                    }

                                    expandoDict.Add(item.Name, sbuilder.ToString());
                                    break;
                                default:

                                    if (item.UseSplit.HasValue && item.UseSplit.Value)
                                    {
                                        string[] value = property.Value.ToString().Split(' ');
                                        int lenght = value.Length / 2;
                                        StringBuilder sb = new StringBuilder();

                                        if (item.positionInitial.HasValue && item.positionInitial.Value)
                                        {
                                            for (int r = 0; r < lenght; r++)
                                            {
                                                sb.Append(value[r]);
                                                sb.Append(" ");
                                            }
                                        }
                                        else
                                        {
                                            for (int r = lenght; r < value.Length; r++)
                                            {
                                                sb.Append(value[r]);
                                                sb.Append(" ");
                                            }
                                        }
                                        expandoDict.Add(item.Name, sb.ToString());
                                    }
                                    else
                                    {
                                        expandoDict.Add(item.Name, property.Value.ToString());
                                    }

                                    break;
                            }
                        }
                    }
                }
            }

            foreach (var fieldNotMaped in fieldsNoMapperCI)
            {
                expandoDict.AddProperty(fieldNotMaped.Name, fieldNotMaped.DefaultValue);
            }

            validationFields = expando.ValidateRequiredFields(fieldTables);
        }

        public static Dictionary<string, object> KeyValuePairs(this object target)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            JToken json = JToken.Parse(JsonConvert.SerializeObject(target));

            JsonFieldsCollector fieldsCollector = new JsonFieldsCollector(json);
            var fields = fieldsCollector.GetAllFields().ToList();

            foreach (var parameter in fields)
            {
                keyValuePairs.Add(parameter.Key, parameter.Value);
            }

            return keyValuePairs;
        }
        public static IEnumerable<string> GetMemberNames(this object target, bool dynamicOnly = false)
        {
            Type ComObjectType = target.GetType();
            dynamic ComBinder = target;

            var prop = target.GetType().GetProperties();

            var tList = new List<string>();

            if (!dynamicOnly)
            {
                tList.AddRange(target.GetType().GetProperties().Select(it => it.Name));
            }

            var tTarget = target as IDynamicMetaObjectProvider;

            if (tTarget != null)
            {
                tList.AddRange(tTarget.GetMetaObject(Expression.Constant(tTarget)).GetDynamicMemberNames());
            }
            else
            {
                if (ComObjectType != null && ComObjectType.IsInstanceOfType(target) && ComBinder.IsAvailable)
                {
                    tList.AddRange(ComBinder.GetDynamicDataMemberNames(target));
                }
            }
            return tList;
        }
    }


    internal class JsonFieldsCollector
    {
        private readonly Dictionary<string, JValue> fields;

        public JsonFieldsCollector(JToken token)
        {
            fields = new Dictionary<string, JValue>();
            CollectFields(token);
        }

        private void CollectFields(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        CollectFields(child);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        CollectFields(child);
                    break;
                case JTokenType.Property:
                    CollectFields(((JProperty)jToken).Value);
                    break;
                default:
                    fields.Add(jToken.Path, (JValue)jToken);
                    break;
            }
        }

        public IEnumerable<KeyValuePair<string, JValue>> GetAllFields() => fields;
    }
}
