using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models.Configuration
{
    public class IntegrationConfiguration
    {
        public string EndpointAdm { get; set; }
        public string TokenAdm { get; set; }
        public string UserServiceNow { get; set; }
        public string PasswordServiceNow { get; set; }
        public string EndpointServiceNow { get; set; }
        public RelationShip Relationship { get; set; }
        public List<Table> Table { get; set; }
    }

    public class RelationShip
    {
        public string Table { get; set; }
        public List<InputRelationShip> ListRelationship { get; set; }
    }
    public class FieldTable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string MapperTo { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public bool NeedId { get; set; }
        public bool Mapper { get; set; }
        public bool? UseSplit { get; set; }
        public bool Required { get; set; }
        public bool? positionInitial { get; set; }
    }

    public class Table
    {
        public string NameReference { get; set; }
        public string Name { get; set; }
        public List<string> SearchBy { get; set; }
        public List<string> TypeHadware { get; set; }
        public List<FieldTable> Fields { get; set; }
    }

}
