using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KsAuthentication.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace KsAuthentication.Jwt
{
    public class JwtToken(JwtOptions jwt)
    {
        private readonly JwtOptions _jwt = jwt;

        public (string Token, DateTime ExpiresAt) CreateJwt(JwtUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (ClaimTypes.Name, user.UserName),
        };
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
        }
    }
}
