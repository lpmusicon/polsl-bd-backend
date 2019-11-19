using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BackendProject;

namespace BackendProject.Helpers
{
  public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private DatabaseContext _db;

        public UserService() {
            this._db = new DatabaseContext();
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _db.Users.SingleOrDefault(x => x.Login == username && x.Password == password));

            // return null if user not found
            if (user == null)
                return null;

            user.Password = null;
            return user;
        }
    }
}