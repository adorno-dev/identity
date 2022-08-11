using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Requests
{
    public class RedefinePasswordRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords mismatching.")]
        public string? ConfirmPassword { get; set; }
    }
}