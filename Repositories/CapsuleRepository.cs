using System;
using System.Threading.Tasks;
using Calmind.Api.Data;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Calmind.Api.Repositories
{
    public class CapsuleRepository : ICapsuleRepository
    {
        private readonly CalmindContext _ctx;
        public CapsuleRepository(CalmindContext ctx) => _ctx = ctx;

        public async Task AddAsync(Capsule capsule) => await _ctx.Capsules.AddAsync(capsule);

        public async Task<Capsule?> GetByIdAsync(Guid id) => await _ctx.Capsules.FirstOrDefaultAsync(c => c.Id == id);

        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
