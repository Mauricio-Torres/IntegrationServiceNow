using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models
{
    public class InputRelationShip
    {
        public string parent { set; get; }
        public string type { set; get; }
        public string child { set; get; }

    }
}
