using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Utils
{

    public enum TypeService
    {
        ManagementCI,
        Conection,
        CI,
        Adm,
        ConfigureRepository
    }
    public static class Constants
    {

        #region Endpoint
        public const string UrlListDevice = "/device";

        #endregion

        #region Connection Service
        public const string ContentType = "Content-Type";
        public const string FormatTypeJson = "application/json";
        public const string Authorization = "Authorization";
        public const string ErrorServer = "ErrorConectionServer";
        public const string NoFound = "NoFound";
        public const string TypeAuthenticationBasic = "Basic ";
        #endregion

        public const string NameHeaderAdm = "X-Authorization";
        public const string CurrentMethod = "CurrentMethod";
        public const string ContentBodyNotAllowed = "ContentBodyNotAllowed";

        #region Search Device Adm
        public const string IdDevice = "idDevice";
        public const string TypeDevice = "TypeDevice";
        public const string Search = "search";
        #endregion

    }
}
