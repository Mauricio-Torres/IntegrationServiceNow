using Aranda.Integration.ServiceNow.Client;
using Aranda.Integration.ServiceNow.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Integration.ServiceNow
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = new IntegrationServiceNow();

            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                // { "TypeDevice", "Laptop"},
                // { "idDevice", "6"}
            };

            var obj =  data.CreateCI(dictionary).Result;
        }
    }
}
