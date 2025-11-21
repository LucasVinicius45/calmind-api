using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calmind.Api.DTOs;

namespace Calmind.Api.Services.Interfaces
{
    public interface IReservationService
    {
        Task<(bool Success, string? Error, ReservationResponseDto? Reservation)> CreateAsync(ReservationCreateDto dto);
        Task<ReservationResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ReservationResponseDto>> GetByCollaboratorAsync(Guid collaboratorId);
        Task<bool> CancelAsync(Guid reservationId);

        // ✅ CORRIGIDO: Retorna TimeSlotDto ao invés de tupla
        Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(
            Guid capsuleId,
            DateTime date,
            TimeSpan workStart,
            TimeSpan workEnd,
            int cleaningMinutes = 0);
    }
}