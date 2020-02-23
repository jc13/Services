using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ToDoServices.Data.Cache
{
    public class Config
    {
        public struct UserRole
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
        public struct CacheKey
        {
            public const string ToDoListGetAll = "ToDoListGetAll";
            public const string UsersGetAll = "UsersGetAll";
        }

        [Obsolete]
        public string SekretKey { get { return ConfigurationSettings.AppSettings["SecretKey"]; } }
    }
}