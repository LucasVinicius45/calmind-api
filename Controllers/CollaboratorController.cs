using System;
using System.Linq;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calmind.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")] // ✅ Apenas Admin pode acessar
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _service;

        public CollaboratorController(ICollaboratorService service) => _service = service;

        /// <summary>
        /// Buscar colaborador por ID (somente Admin)
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var c = await _service.GetByIdAsync(id);
            if (c == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Colaborador não encontrado"));

            // Não retornar o hash da senha
            var response = new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.Role,
                c.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResponse(response));
        }

        /// <summary>
        /// Buscar colaborador por email (somente Admin)
        /// </summary>
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var c = await _service.GetByEmailAsync(email);
            if (c == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Colaborador não encontrado"));

            var response = new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.Role,
                c.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResponse(response));
        }
    }
}