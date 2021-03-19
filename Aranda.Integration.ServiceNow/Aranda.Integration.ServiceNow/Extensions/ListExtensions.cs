using Aranda.Integration.ServiceNow.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Extensions
{
    internal static class ListExtensions
    {
        public static List<FieldTable> AddReferences(this List<FieldTable> fieldsWithReference, List<FieldTable> fields)
        {
            var references = fields.Where(reference => !string.IsNullOrWhiteSpace(reference.Reference)).ToList();
             fieldsWithReference.AddRange(references);
            return fieldsWithReference;
        }

        public static List<object> Data(this List<DataRelationship> dataRelationships)
        {
            List<object> list = new List<object>();

            foreach(var data in dataRelationships)
            {
                foreach (var obj in data.Data)
                {
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}
