using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calmind.Api.Models;

namespace Calmind.Api.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation r);
        Task<int> CountConfirmedForCollaboratorInRangeAsync(Guid collaboratorId, DateTime from, DateTime to);
        Task<bool> HasCapsuleConflictAsync(Guid capsuleId, DateTime start, DateTime end);
        Task SaveChangesAsync();
        Task<Reservation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> GetByCollaboratorAsync(Guid collaboratorId);
        Task<IEnumerable<Reservation>> GetByCapsuleAndDateAsync(Guid capsuleId, DateTime date);
        Task UpdateAsync(Reservation reservation);
    }
}
