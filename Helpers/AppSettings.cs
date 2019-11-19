using System.Collections.Generic;
using System.Linq;

using BackendProject.Interface;

namespace BackendProject.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
    }

    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users) {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user) {
            user.Password = null;
            return user;
        }
    }
}