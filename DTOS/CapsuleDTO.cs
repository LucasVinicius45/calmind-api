using System;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.DTOs
{
    public class CapsuleDto
    {
        public Guid Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int DurationMinutes { get; set; } = 40;
        public string Features { get; set; } = "";
    }
}
