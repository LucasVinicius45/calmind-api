using System.Linq;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Services;
using Calmind.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Calmind.Api.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registrar novo usuário
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));

            var (success, error, response) = await _authService.RegisterAsync(dto);

            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse(error!));

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response!, "Usuário registrado com sucesso"));
        }

        /// <summary>
        /// Login de usuário
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));

            var (success, error, response) = await _authService.LoginAsync(dto);

            if (!success)
                return Unauthorized(ApiResponse<object>.ErrorResponse(error!));

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response!, "Login realizado com sucesso"));
        }
    }
}