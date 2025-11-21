using System;

namespace Calmind.Api.DTOs
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid CollaboratorId { get; set; }
        public Guid CapsuleId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
