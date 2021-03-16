using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Utils
{
    public class CustomException : Exception
    {
        public CustomException(string message)
   : base(message) { }

        public CustomException(string message, Exception inner)
            : base(message, inner) { }

        public string ErrorMessage { get; }
    }
}
