using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Calmind.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<object>.ErrorResponse(errors));
            }

            var (success, error, reservation) = await _service.CreateAsync(dto);

            if (!success)
            {
                if (error?.Contains("Horário já reservado") == true)
                    return Conflict(ApiResponse<object>.ErrorResponse(error));
                return BadRequest(ApiResponse<object>.ErrorResponse(error!));
            }

            return CreatedAtAction(nameof(GetById), new { id = reservation!.Id },
                ApiResponse<ReservationResponseDto>.SuccessResponse(reservation, "Reserva criada com sucesso"));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var r = await _service.GetByIdAsync(id);
            if (r == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Reserva não encontrada"));

            return Ok(ApiResponse<ReservationResponseDto>.SuccessResponse(r));
        }

        [HttpGet("collaborator/{collaboratorId:guid}")]
        public async Task<IActionResult> GetByCollaborator(Guid collaboratorId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != collaboratorId.ToString())
                return Forbid();

            var list = await _service.GetByCollaboratorAsync(collaboratorId);
            return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.SuccessResponse(list));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var reservation = await _service.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Reserva não encontrada"));

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != reservation.CollaboratorId.ToString())
                return Forbid();

            var ok = await _service.CancelAsync(id);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Reserva não encontrada"));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Reserva cancelada com sucesso"));
        }

        [HttpGet("availability/{capsuleId:guid}")]
        public async Task<IActionResult> Availability(
            Guid capsuleId,
            [FromQuery] DateTime date,
            [FromQuery] string workStart = "08:00",
            [FromQuery] string workEnd = "18:00",
            [FromQuery] int cleaningMinutes = 0)
        {
            if (date == default)
                return BadRequest(ApiResponse<object>.ErrorResponse("data é obrigatória"));

            if (!TimeSpan.TryParse(workStart, out var startTs)) startTs = TimeSpan.FromHours(8);
            if (!TimeSpan.TryParse(workEnd, out var endTs)) endTs = TimeSpan.FromHours(18);

            var slots = await _service.GetAvailableSlotsAsync(capsuleId, date.Date, startTs, endTs, cleaningMinutes);
            return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.SuccessResponse(slots));
        }
    }
}