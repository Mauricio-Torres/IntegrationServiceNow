using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Utils
{
    public static class GenericPropertyEntity<TModel> where TModel : class
    {
        public static Dictionary<string, object> ModelProperty(TModel tmodelObj)
        {
            Type tModelType = tmodelObj.GetType();
            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            foreach(var item in arrayPropertyInfos)
            {
                keyValuePairs.Add(item.Name, item.GetValue(tmodelObj));
            }

            return keyValuePairs;
        }
    }
}
