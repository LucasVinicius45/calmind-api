using System;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Calmind.Api.Services.Interfaces;

namespace Calmind.Api.Services
{
    public class CapsuleService : ICapsuleService
    {
        private readonly ICapsuleRepository _repo;

        public CapsuleService(ICapsuleRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Success, string? Error, Capsule? Capsule)> CreateAsync(CapsuleDto dto)
        {
            var capsule = new Capsule
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name,
                Location = dto.Location,
                IsActive = dto.IsActive,
                DurationMinutes = dto.DurationMinutes,
                Features = dto.Features
            };

            await _repo.AddAsync(capsule);
            await _repo.SaveChangesAsync();

            return (true, null, capsule);
        }

        public async Task<Capsule?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}