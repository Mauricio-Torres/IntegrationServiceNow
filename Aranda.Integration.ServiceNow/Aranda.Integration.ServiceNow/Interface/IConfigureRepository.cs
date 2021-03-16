using Aranda.Integration.ServiceNow.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface IConfigureRepository
    {
        IntegrationConfiguration GetConfiguration();
    }
}
