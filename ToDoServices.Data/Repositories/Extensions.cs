using System;
using ToDoServices.Data.Cache;
using ToDoServices.Data.Models;

namespace ToDoServices.Data.Repositories
{
    public static class Extensions
    {
        public static bool IsAdmin(this User appUser)
        {
            if (appUser == null)
                return false;
            else
                return (appUser.Role ?? Config.UserRole.User) == Config.UserRole.Admin;
        }

        public static bool IsOwner(this User appUser, ToDoList list)
        {
            if (appUser == null
                || list == null
                || string.IsNullOrWhiteSpace(appUser.UserName)
                || string.IsNullOrWhiteSpace(list.Author))
                return false;

            return appUser.UserName.Equals(list.Author, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
