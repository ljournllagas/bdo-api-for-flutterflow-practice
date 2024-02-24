using Application.DTOs.Authentication.Response;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IAuthenticateService
    {
        UserToken BuildToken(string username);
    }
}
