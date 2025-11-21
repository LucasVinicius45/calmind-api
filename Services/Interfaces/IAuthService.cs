using System.Threading.Tasks;
using Calmind.Api.DTOs;

namespace Calmind.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string? Error, AuthResponseDto? Response)> LoginAsync(LoginDto dto);
    }
}