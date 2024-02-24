using System;

namespace Application.DTOs.Authentication.Response
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}