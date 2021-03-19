using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi
{
    internal class ResponseGeneralSysId
    {
        public SysId result { get; set; }
    }

    internal class SysId
    {
        public string name { get; set; }
        public string sys_id { get; set; }
    }

}
