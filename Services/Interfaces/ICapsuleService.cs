using System;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Models;

namespace Calmind.Api.Services.Interfaces
{
    public interface ICapsuleService
    {
        Task<(bool Success, string? Error, Capsule? Capsule)> CreateAsync(CapsuleDto dto);
        Task<Capsule?> GetByIdAsync(Guid id);
    }
}