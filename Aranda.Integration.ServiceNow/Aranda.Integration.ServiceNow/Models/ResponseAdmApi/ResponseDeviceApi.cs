using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models.ResponseAdmApi
{
    internal class ResponseDeviceApi
    {
        public List<Device> Content { get; set; }
        public int TotalItems { get; set; }
    }


    internal class Device
    {
        public string AgentProfile { get; set; }
        public string AgentVersion { get; set; }
        public string CompleteIpAddress { get; set; }
        public DateTime CreationDate { get; set; }
        public int? DaysSinceLastInventory { get; set; }
        public string Description { get; set; }
        public bool Discovery { get; set; }
        public double DiskUsage { get; set; }
        public string Domain { get; set; }
        public int Id { get; set; }
        public string IpRegistred { get; set; }
        public DateTime? LastInventory { get; set; }
        public string Manufacturer { get; set; }
        public double MemoryUsage { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string ResponsibleUserEmail { get; set; }
        public int? ResponsibleUserId { get; set; }
        public string ResponsibleUserName { get; set; }
        public string Serial { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Virtualization { get; set; }
        public bool Vpro { get; set; }
    }
}
