using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calmind.Api.Data;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Calmind.Api.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly CalmindContext _ctx;
        public ReservationRepository(CalmindContext ctx) => _ctx = ctx;

        public async Task AddAsync(Reservation r) => await _ctx.Reservations.AddAsync(r);

        public async Task<int> CountConfirmedForCollaboratorInRangeAsync(Guid collaboratorId, DateTime from, DateTime to) =>
            await _ctx.Reservations
                .Where(r => r.CollaboratorId == collaboratorId && r.Status == ReservationStatus.Confirmed && r.StartAt >= from && r.StartAt < to)
                .CountAsync();

        public async Task<bool> HasCapsuleConflictAsync(Guid capsuleId, DateTime start, DateTime end) =>
            await _ctx.Reservations.AnyAsync(r => r.CapsuleId == capsuleId && r.Status == ReservationStatus.Confirmed && r.StartAt < end && r.EndAt > start);

        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public async Task<Reservation?> GetByIdAsync(Guid id) =>
            await _ctx.Reservations.Include(r => r.Capsule).Include(r => r.Collaborator).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Reservation>> GetByCollaboratorAsync(Guid collaboratorId) =>
            await _ctx.Reservations.Where(r => r.CollaboratorId == collaboratorId).OrderByDescending(r => r.StartAt).ToListAsync();

        public async Task<IEnumerable<Reservation>> GetByCapsuleAndDateAsync(Guid capsuleId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return await _ctx.Reservations
                .Where(r => r.CapsuleId == capsuleId && r.StartAt >= start && r.StartAt < end && r.Status == ReservationStatus.Confirmed)
                .ToListAsync();
        }

        public Task UpdateAsync(Reservation reservation)
        {
            _ctx.Reservations.Update(reservation);
            return Task.CompletedTask;
        }
    }
}
