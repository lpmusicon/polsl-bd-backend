using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BackendProject;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BackendProject.Helpers
{
  public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }

    public class PasswordHelper
    {
        public string CreateHashedPassword(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, 20);
            byte[] salt = deriveBytes.Salt;
            byte[] key = deriveBytes.GetBytes(32);  // derive a 20-byte key

            return string.Format("{0}.{1}", Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public bool CompareHashedPassword(string saltKey, string rawPassword)
        {
            byte[] salt = Convert.FromBase64String(saltKey.Split('.').FirstOrDefault());
            byte[] key = Convert.FromBase64String(saltKey.Split('.').LastOrDefault());
            using var deriveBytes = new Rfc2898DeriveBytes(rawPassword, salt);
            byte[] newKey = deriveBytes.GetBytes(32);  // derive a 20-byte key
            return newKey.SequenceEqual(key);
        }
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly DatabaseContext _db;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings) {
            this._db = new DatabaseContext();
            _appSettings = appSettings.Value;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _db.Users.SingleOrDefault(x => x.Login == username));
            var pw = new PasswordHelper();

            if (user == null)
                return null;

            if(!pw.CompareHashedPassword(user.Password, password))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            
            user.Password = null;
            
            return user;
        }
    }
}