using System;
using System.Threading.Tasks;
using Calmind.Api.Models;

namespace Calmind.Api.Repositories.Interfaces
{
    public interface ICapsuleRepository
    {
        Task<Capsule?> GetByIdAsync(Guid id);
        Task AddAsync(Capsule capsule);
        Task SaveChangesAsync();
    }
}
