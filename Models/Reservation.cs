using System;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.Models
{
    public enum ReservationStatus { Confirmed, Cancelled, Completed }

    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CollaboratorId { get; set; }
        public Collaborator? Collaborator { get; set; }

        [Required]
        public Guid CapsuleId { get; set; }
        public Capsule? Capsule { get; set; }

        [Required]
        public DateTime StartAt { get; set; } // UTC

        [Required]
        public DateTime EndAt { get; set; } // StartAt + DurationMinutes

        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
