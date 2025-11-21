using System;
using System.Threading.Tasks;
using Calmind.Api.DTOs;
using Calmind.Api.Models;
using Calmind.Api.Repositories.Interfaces;
using Calmind.Api.Services.Interfaces;

namespace Calmind.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICollaboratorRepository _collabRepo;
        private readonly JwtService _jwtService;

        public AuthService(ICollaboratorRepository collabRepo, JwtService jwtService)
        {
            _collabRepo = collabRepo;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string? Error, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto)
        {
            var existing = await _collabRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return (false, "Email já cadastrado", null);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var collaborator = new Collaborator
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role
            };

            await _collabRepo.AddAsync(collaborator);
            await _collabRepo.SaveChangesAsync();

            var token = _jwtService.GenerateToken(collaborator);
            var expiresAt = _jwtService.GetTokenExpiration();

            var response = new AuthResponseDto
            {
                Id = collaborator.Id,
                FullName = collaborator.FullName,
                Email = collaborator.Email,
                Role = collaborator.Role,
                Token = token,
                ExpiresAt = expiresAt
            };

            return (true, null, response);
        }

        public async Task<(bool Success, string? Error, AuthResponseDto? Response)> LoginAsync(LoginDto dto)
        {
            var collaborator = await _collabRepo.GetByEmailAsync(dto.Email);
            if (collaborator == null)
                return (false, "Email ou senha inválidos", null);

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, collaborator.PasswordHash);
            if (!isPasswordValid)
                return (false, "Email ou senha inválidos", null);

            var token = _jwtService.GenerateToken(collaborator);
            var expiresAt = _jwtService.GetTokenExpiration();

            var response = new AuthResponseDto
            {
                Id = collaborator.Id,
                FullName = collaborator.FullName,
                Email = collaborator.Email,
                Role = collaborator.Role,
                Token = token,
                ExpiresAt = expiresAt
            };

            return (true, null, response);
        }
    }
}