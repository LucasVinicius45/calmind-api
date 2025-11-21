using System;
using System.Threading.Tasks;
using Calmind.Api.Models;

namespace Calmind.Api.Repositories.Interfaces
{
    public interface ICollaboratorRepository
    {
        Task<Collaborator?> GetByIdAsync(Guid id);
        Task<Collaborator?> GetByEmailAsync(string email);
        Task AddAsync(Collaborator c);
        Task SaveChangesAsync();
    }
}
