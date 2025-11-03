using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunityFunctions.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateCustomerToken(string name, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("name", name),
                new Claim("role", "customer"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateOrganiserToken(Guid organiserId, string name)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, $"user-{organiserId}"),
            new Claim("name", name),
            new Claim("role", "organiser"),
            new Claim("organiserId", organiserId.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public string GenerateStaffToken(string name)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, $"user-{Guid.NewGuid()}"),
            new Claim("name", name),
            new Claim("role", "staff")
        }),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
        //public string GenerateOrganiserToken(Guid organiserId, string name)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, $"user-{organiserId}"),
        //        new Claim("name", name),
        //        new Claim("role", "organiser"),
        //        new Claim("organiserId", organiserId.ToString()),
        //        new Claim(JwtRegisteredClaimNames.Iss, _issuer),
        //        new Claim(JwtRegisteredClaimNames.Aud, _audience)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _issuer,
        //        audience: _audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(3),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //public string GenerateStaffToken(string name)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_secretKey);

        //    var descriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("sub", Guid.NewGuid().ToString()),
        //            new Claim("name", name),
        //            new Claim("role", "staff")
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(6),
        //        Issuer = _issuer,
        //        Audience = _audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = handler.CreateToken(descriptor);
        //    return handler.WriteToken(token);
        //}
    }
}
