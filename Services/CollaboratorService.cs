using System;
using System.Threading.Tasks;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Calmind.Api.Services.Interfaces;

namespace Calmind.Api.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _repo;

        public CollaboratorService(ICollaboratorRepository repo)
        {
            _repo = repo;
        }

        public async Task<Collaborator?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Collaborator?> GetByEmailAsync(string email)
        {
            return await _repo.GetByEmailAsync(email);
        }
    }
}