using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Models.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Repository
{
    internal class ConfigureRepository : IConfigureRepository
    {
        public IntegrationConfiguration GetConfiguration()
        {
            string path = @"C:\Users\Mauricio\Desktop\ARANDA\Service now\Aranda.Integration.ServiceNow\Aranda.Integration.ServiceNow\Repository\configuration.json";
            IntegrationConfiguration configuration = new IntegrationConfiguration();
            using (StreamReader jsonStream = File.OpenText(path))
            {
                var json = jsonStream.ReadToEnd();
                configuration = JsonConvert.DeserializeObject<IntegrationConfiguration>(json);
            }
            return configuration;
        }
    }
}
