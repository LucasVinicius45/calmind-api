using System;
using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.DTOs
{
    public class CollaboratorDto
    {
        public Guid Id { get; set; }
        [Required] public string FullName { get; set; } = null!;
        [Required] public string Email { get; set; } = null!;
        public string Role { get; set; } = "User";
    }
}
