using EmployeeProjectManagements.UI.Consts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeProjectManagements.UI.Manager
{
	public class EMPManager
	{
		private readonly IConfiguration _configuration;
        public EMPManager(IConfiguration configuration)
        {
				_configuration = configuration;
        }
        public string CreateToken(string userName,string password)
		{
			try
			{
				var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value));
				var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
				var tokenValidate = Guid.NewGuid().ToString();

				var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
			new Claim(ClaimConst.TokenValidKey, tokenValidate),
			new Claim(ClaimConst.UserName,userName),
			new Claim(ClaimConst.Password, password),
				};
				var token = new JwtSecurityToken(_configuration.GetSection("JWT:Issuers").Value,
					_configuration.GetSection("JWT:Audience").Value, claims, expires: DateTime.Now.AddMinutes(10), signingCredentials: credentials);
				return new JwtSecurityTokenHandler().WriteToken(token);
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}

	}
}
