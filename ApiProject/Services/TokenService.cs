﻿using ApiProject.Models;
using ApiProject.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiProject.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _config; // to accsess appsettings.json
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"])); 
                
        }
        public string CreateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.Email,user.Email),
               new Claim(JwtRegisteredClaimNames.GivenName,user.UserName)
           };

            SigningCredentials creds = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(7),
                SigningCredentials=creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescription);  

            return tokenHandler.WriteToken(token);  

        }
    }
}
