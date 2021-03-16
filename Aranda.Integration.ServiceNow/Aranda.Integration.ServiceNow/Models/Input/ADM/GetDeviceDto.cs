using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models.Input.ADM
{
    internal class GetDeviceDto
    {
        public string search { set; get; }
        public string orderField { set; get; }
        public string orderType { set; get; }
        public int pageSize { set; get; }
        public int pageIndex { set; get; }
    }
}
