using Application.DTOs.Authentication.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Authentication.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;

        public AuthenticateService(IOptions<JWTSettings> jwtSettings,
            IDateTimeService dateTimeService)
        {
            _jwtSettings = jwtSettings.Value;
            _dateTimeService = dateTimeService;
        }

        public UserToken BuildToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = _dateTimeService.Now.AddMinutes(_jwtSettings.DurationInMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }


    }

}
