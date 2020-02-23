using ToDoServices.Data.Models;

namespace ToDoServices.Data.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(string userName);
    }
}
