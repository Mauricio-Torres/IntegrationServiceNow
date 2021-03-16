using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aranda.Integration.ServiceNow.Extensions
{
    internal static class StringExtension
    {
        public static string ConvertUrl(this string url, Dictionary<string, object> search)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
            
            foreach (var value in search)
            {
                if (value.Key !=null && value.Value !=null && !string.IsNullOrEmpty( value.Value.ToString()))
                {
                    httpValueCollection.Add(value.Key, value.Value.ToString());
                }
            }

            UriBuilder uri = new UriBuilder(url);
            uri.Query = httpValueCollection.ToString();

            return uri.ToString();
        }
    }
}