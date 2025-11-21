using System;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.DTOs
{
    public class ReservationCreateDto
    {
        [Required]
        public Guid CollaboratorId { get; set; }

        [Required]
        public Guid CapsuleId { get; set; }

        [Required]
        public DateTime StartAtUtc { get; set; } // expect UTC from client
    }
}
