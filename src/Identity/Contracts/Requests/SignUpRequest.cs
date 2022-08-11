using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Requests
{
    public class SignUpRequest
    {
        [Required]
        public string? Fullname { get; set; }     
        [Required]
        public string? Username { get; set; }        
        [Required]
        public string? Email { get; set; }
        [Required]
        [StringLength(11)]
        public string? CPF { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}