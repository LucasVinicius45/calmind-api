using System;
using System.Threading.Tasks;
using Calmind.Api.Data;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Calmind.Api.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly CalmindContext _ctx;
        public CollaboratorRepository(CalmindContext ctx) => _ctx = ctx;

        public async Task AddAsync(Collaborator c) => await _ctx.Collaborators.AddAsync(c);

        public async Task<Collaborator?> GetByEmailAsync(string email) => await _ctx.Collaborators.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<Collaborator?> GetByIdAsync(Guid id) => await _ctx.Collaborators.FirstOrDefaultAsync(x => x.Id == id);

        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
