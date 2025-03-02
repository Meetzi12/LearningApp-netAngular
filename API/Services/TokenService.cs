using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found in appsettings.json");
        //check length
        if(tokenKey.Length < 64) throw new Exception("TokenKey must be at least 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        //create claims
        var claims = new List<Claim>{
            new (ClaimTypes.NameIdentifier, user.UserName)
        };

        //create credentials
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    
        //create token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        //create token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        //create token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }

}
