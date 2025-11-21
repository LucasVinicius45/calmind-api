using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.Models
{
    public class Collaborator
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!; // ✅ NOVO: armazenar hash da senha

        [Required]
        public string Role { get; set; } = "User"; // "User" ou "Admin"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}