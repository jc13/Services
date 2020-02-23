using System;
using System.Collections.Generic;
using System.Linq;
using ToDoServices.Data.Cache;
using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;

namespace ToDoServices.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ICacheService _cache;
        public UserRepository(ICacheService cache)
        {
            _cache = cache;
        }

        public List<User> UserList
        {
            get
            {
                return _cache.GetOrSet(Config.CacheKey.UsersGetAll, () =>
                   new List<User>()
                   {
                        new User { Id = 100, UserName = "Admin", Role=Config.UserRole.Admin},
                        new User { Id = 200, UserName = "Brian", Role=Config.UserRole.User},
                        new User { Id = 300, UserName = "Marry", Role=Config.UserRole.User}
                   }
                );
            }
        }

        public User GetUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            var u = UserList.SingleOrDefault(x=> 
                x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

            return u;
        }

    }
}
