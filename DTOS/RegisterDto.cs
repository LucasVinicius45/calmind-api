using System.ComponentModel.DataAnnotations;

namespace Calmind.Api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [MaxLength(200)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = "User"; // User ou Admin
    }
}