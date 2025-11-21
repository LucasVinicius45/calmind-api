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
    public class CapsuleController : ControllerBase
    {
        private readonly ICapsuleService _service;

        public CapsuleController(ICapsuleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Criar cápsula (somente Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CapsuleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var (success, error, capsule) = await _service.CreateAsync(dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(error!));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = capsule!.Id },
                ApiResponse<Models.Capsule>.SuccessResponse(capsule, "Cápsula criada com sucesso")
            );
        }

        /// <summary>
        /// Buscar cápsula por ID (qualquer usuário autenticado)
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cap = await _service.GetByIdAsync(id);

            if (cap == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Cápsula não encontrada"));
            }

            return Ok(ApiResponse<Models.Capsule>.SuccessResponse(cap));
        }
    }
}