using System.ComponentModel.DataAnnotations;

namespace JWTAuthAuthentication.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage ="O email deve ser válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Informe a senha")]
        public string Password { get; set; }
    }
}
