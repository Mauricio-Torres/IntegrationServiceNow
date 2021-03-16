using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Models.ResponseServiceNowApi
{
    public class CreateCi
    {
        public string os_address_width { get; set; }
        public string attested_date { get; set; }
        public string operational_status { get; set; }
        public string os_service_pack { get; set; }
        public string cpu_core_thread { get; set; }
        public string cpu_manufacturer { get; set; }
        public string sys_updated_on { get; set; }
        public string discovery_source { get; set; }
        public string first_discovered { get; set; }
        public string due_in { get; set; }
        public string gl_account { get; set; }
        public string invoice_number { get; set; }
        public string sys_created_by { get; set; }
        public string ram { get; set; }
        public string warranty_expiration { get; set; }
        public string cpu_name { get; set; }
        public string cpu_speed { get; set; }
        public string owned_by { get; set; }
        public string checked_out { get; set; }
        public string disk_space { get; set; }
        public string sys_domain_path { get; set; }
        public string object_id { get; set; }
        public string maintenance_schedule { get; set; }
        public string cost_center { get; set; }
        public string attested_by { get; set; }
        public string dns_domain { get; set; }
        public string assigned { get; set; }
        public string life_cycle_stage { get; set; }
        public string purchase_date { get; set; }
        public string cd_speed { get; set; }
        public string short_description { get; set; }
        public string floppy { get; set; }
        public string managed_by { get; set; }
        public string os_domain { get; set; }
        public string can_print { get; set; }
        public string last_discovered { get; set; }
        public string sys_class_name { get; set; }
        public string cpu_count { get; set; }
        public ResponseGeneralApi manufacturer { get; set; }
        public string life_cycle_stage_status { get; set; }
        public string vendor { get; set; }
        public string model_number { get; set; }
        public ResponseGeneralApi assigned_to { get; set; }
        public string start_date { get; set; }
        public string os_version { get; set; }
        public string serial_number { get; set; }
        public string cd_rom { get; set; }
        public string support_group { get; set; }
        public string correlation_id { get; set; }
        public string unverified { get; set; }
        public string attributes { get; set; }
        public object asset { get; set; }
        public string cpu_core_count { get; set; }
        public string form_factor { get; set; }
        public string skip_sync { get; set; }
        public string attestation_score { get; set; }
        public string sys_updated_by { get; set; }
        public string sys_created_on { get; set; }
        public string cpu_type { get; set; }
        public ResponseGeneralApi sys_domain { get; set; }
        public string install_date { get; set; }
        public string asset_tag { get; set; }
        public string hardware_substatus { get; set; }
        public string fqdn { get; set; }
        public string change_control { get; set; }
        public string internet_facing { get; set; }
        public string delivery_date { get; set; }
        public string hardware_status { get; set; }
        public string install_status { get; set; }
        public string supported_by { get; set; }
        public string name { get; set; }
        public string u_name_owner { get; set; }
        public string subcategory { get; set; }
        public string default_gateway { get; set; }
        public string chassis_type { get; set; }
        public bool @virtual { get; set; }
        public string assignment_group { get; set; }
        public string managed_by_group { get; set; }
        public string sys_id { get; set; }
        public string po_number { get; set; }
        public string checked_in { get; set; }
        public string sys_class_path { get; set; }
        public string mac_address { get; set; }
        public ResponseGeneralApi company { get; set; }
        public string justification { get; set; }
        public ResponseGeneralApi department { get; set; }
        public string comments { get; set; }
        public string cost { get; set; }
        public string os { get; set; }
        public string sys_mod_count { get; set; }
        public string monitor { get; set; }
        public string ip_address { get; set; }
        public ResponseGeneralApi model_id { get; set; }
        public string duplicate_of { get; set; }
        public string sys_tags { get; set; }
        public string cost_cc { get; set; }
        public string order_date { get; set; }
        public string schedule { get; set; }
        public string environment { get; set; }
        public string due { get; set; }
        public string attested { get; set; }
        public string location { get; set; }
        public string category { get; set; }
        public string fault_count { get; set; }
        public string lease_id { get; set; }
    }
    public class ResponseCreateCIApi
    {
        public CreateCi result { get; set; }
    }
}
