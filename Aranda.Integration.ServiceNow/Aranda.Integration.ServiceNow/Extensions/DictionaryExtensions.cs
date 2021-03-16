using Aranda.Integration.ServiceNow.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Table = Aranda.Integration.ServiceNow.Models.Configuration.Table;

namespace Aranda.Integration.ServiceNow.Extensions
{
    internal static class DictionaryExtensions
    {
        private static object GetValueProperty(this Dictionary<string, object> o, string member)
        {
            return o.FirstOrDefault(x => x.Key.Equals(member, StringComparison.InvariantCultureIgnoreCase)).Value;
        }
        public static string GetKeyProperty(this Dictionary<string, object> o, string member)
        {
            return o.FirstOrDefault(x => x.Key.Equals(member, StringComparison.InvariantCultureIgnoreCase)).Key;
        }
        public static Dictionary<string, object> SetValueProperty(this Dictionary<string, object> valueSearch, Dictionary<string, object> propertyDevice, Table tableReference)
        {
            foreach (var item in tableReference.SearchBy)
            {
                FieldTable fieldTable = tableReference.Fields.FirstOrDefault(x => x.Name.Equals(item, StringComparison.InvariantCultureIgnoreCase));

                if (fieldTable != null)
                {
                    var value = propertyDevice.GetValueProperty(fieldTable.MapperTo);
                    valueSearch.Add(fieldTable.Name, value);
                }
            }

            return valueSearch;
        }

    }
}
