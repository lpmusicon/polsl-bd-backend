using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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

    public class UserService : IUserService
    {
        private readonly DatabaseContext _db;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings) {
            _db = new DatabaseContext();
            _appSettings = appSettings.Value;
        }

        public async Task<User> Authenticate(string login, string password)
        {
            var user = await Task.Run(() => _db.Users.SingleOrDefault(x => x.Login == login));
            var pw = new PasswordHelper();

            if (user == null)
                return null;

            if(!pw.CompareHashedPassword(user.Password, password))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            
            return user.WithoutPassword();
        }
    }
}