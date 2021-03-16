using Aranda.Integration.ServiceNow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Interface
{
    internal interface IFactory 
    {
        IManagementCIService Create(TypeService typeService);
    }
}
