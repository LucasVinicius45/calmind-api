using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Calmind.Api.Services.Interfaces;

namespace Calmind.Api.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly ICapsuleRepository _capsuleRepo;
        private readonly ICollaboratorRepository _collabRepo;

        public ReservationService(
            IReservationRepository reservationRepo,
            ICapsuleRepository capsuleRepo,
            ICollaboratorRepository collabRepo)
        {
            _reservationRepo = reservationRepo;
            _capsuleRepo = capsuleRepo;
            _collabRepo = collabRepo;
        }

        public async Task<(bool Success, string? Error, ReservationResponseDto? Reservation)> CreateAsync(ReservationCreateDto dto)
        {
            var start = dto.StartAtUtc.ToUniversalTime();

            var capsule = await _capsuleRepo.GetByIdAsync(dto.CapsuleId);
            if (capsule == null || !capsule.IsActive)
                return (false, "Cápsula inválida ou inativa.", null);

            var collab = await _collabRepo.GetByIdAsync(dto.CollaboratorId);
            if (collab == null)
                return (false, "Colaborador não encontrado.", null);

            var end = start.AddMinutes(capsule.DurationMinutes);

            if (await _reservationRepo.HasCapsuleConflictAsync(dto.CapsuleId, start, end))
                return (false, "Horário já reservado nesta cápsula.", null);

            // Semana: segunda → domingo
            int diff = (7 + ((int)start.DayOfWeek) - (int)DayOfWeek.Monday) % 7;
            DateTime weekStart = start.Date.AddDays(-diff);
            DateTime weekEnd = weekStart.AddDays(7);

            var count = await _reservationRepo
                .CountConfirmedForCollaboratorInRangeAsync(dto.CollaboratorId, weekStart, weekEnd);

            if (count >= 2)
                return (false, "Limite de 2 reservas por semana atingido.", null);

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CollaboratorId = dto.CollaboratorId,
                CapsuleId = dto.CapsuleId,
                StartAt = start,
                EndAt = end,
                Status = ReservationStatus.Confirmed
            };

            await _reservationRepo.AddAsync(reservation);
            await _reservationRepo.SaveChangesAsync();

            var resp = new ReservationResponseDto
            {
                Id = reservation.Id,
                CollaboratorId = reservation.CollaboratorId,
                CapsuleId = reservation.CapsuleId,
                StartAt = reservation.StartAt,
                EndAt = reservation.EndAt,
                Status = reservation.Status.ToString()
            };

            return (true, null, resp);
        }

        public async Task<ReservationResponseDto?> GetByIdAsync(Guid id)
        {
            var r = await _reservationRepo.GetByIdAsync(id);
            if (r == null) return null;

            return new ReservationResponseDto
            {
                Id = r.Id,
                CollaboratorId = r.CollaboratorId,
                CapsuleId = r.CapsuleId,
                StartAt = r.StartAt,
                EndAt = r.EndAt,
                Status = r.Status.ToString()
            };
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetByCollaboratorAsync(Guid collaboratorId)
        {
            var list = await _reservationRepo.GetByCollaboratorAsync(collaboratorId);

            return list.Select(r => new ReservationResponseDto
            {
                Id = r.Id,
                CollaboratorId = r.CollaboratorId,
                CapsuleId = r.CapsuleId,
                StartAt = r.StartAt,
                EndAt = r.EndAt,
                Status = r.Status.ToString()
            });
        }

        public async Task<bool> CancelAsync(Guid reservationId)
        {
            var r = await _reservationRepo.GetByIdAsync(reservationId);
            if (r == null) return false;

            r.Status = ReservationStatus.Cancelled;

            await _reservationRepo.UpdateAsync(r);
            await _reservationRepo.SaveChangesAsync();
            return true;
        }

        // ✅ CORRIGIDO: Agora retorna objetos serializáveis ao invés de tuplas
        public async Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(
            Guid capsuleId,
            DateTime date,
            TimeSpan workStart,
            TimeSpan workEnd,
            int cleaningMinutes = 0)
        {
            var capsule = await _capsuleRepo.GetByIdAsync(capsuleId);
            if (capsule == null || !capsule.IsActive)
                return Enumerable.Empty<TimeSlotDto>();

            var reservations = (await _reservationRepo.GetByCapsuleAndDateAsync(capsuleId, date.Date))
                .Select(r => (Start: r.StartAt, End: r.EndAt))
                .OrderBy(x => x.Start)
                .ToList();

            var slots = new List<TimeSlotDto>();

            DateTime cursor = date.Date.Add(workStart);
            DateTime dayEnd = date.Date.Add(workEnd);

            while (cursor.AddMinutes(capsule.DurationMinutes) <= dayEnd)
            {
                var slotStart = cursor;
                var slotEnd = cursor.AddMinutes(capsule.DurationMinutes);

                bool conflict = reservations.Any(e => e.Start < slotEnd && e.End > slotStart);

                if (!conflict)
                {
                    slots.Add(new TimeSlotDto
                    {
                        Start = slotStart,
                        End = slotEnd
                    });
                    cursor = slotEnd.AddMinutes(cleaningMinutes);
                }
                else
                {
                    var nextEnd = reservations
                        .Where(e => e.Start < slotEnd && e.End > slotStart)
                        .Select(e => e.End)
                        .Max();

                    cursor = nextEnd;
                }
            }

            return slots;
        }
    }
}