using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.Models
{
    public class Capsule
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int DurationMinutes { get; set; } = 40;

        [MaxLength(500)]
        public string Features { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
