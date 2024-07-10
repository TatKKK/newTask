using newTask.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace newTask.Auth
{
    public interface IJwtManager
    {
        Token GetToken(User user);
    }
    public class JwtManager:IJwtManager
    {
        private readonly IConfiguration config;

        public JwtManager(IConfiguration config)
        {
            this.config = config;
        }


        public Token GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.UTF8.GetBytes(config["JWT:key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString(), ClaimValueTypes.Integer),
                new Claim("Username", user.Username, ClaimValueTypes.String),
                new Claim("CompanyId", user.CompanyId.ToString(), ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Role, user.Role.Name, ClaimValueTypes.String), 
                new Claim("RoleId", user.RoleId.ToString(), ClaimValueTypes.Integer) 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = signingCredentials
            };
            var tokenData = tokenHandler.CreateToken(tokenDescriptor);
            return new Token { AccessToken = tokenHandler.WriteToken(tokenData) };
        }
    }

    public class Token
    {
        public string? AccessToken { get; set; }
    }
}
