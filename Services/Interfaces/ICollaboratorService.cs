using System;
using System.Threading.Tasks;
using Calmind.Api.Models;

namespace Calmind.Api.Services.Interfaces
{
    public interface ICollaboratorService
    {
        Task<Collaborator?> GetByIdAsync(Guid id);
        Task<Collaborator?> GetByEmailAsync(string email);
    }
}