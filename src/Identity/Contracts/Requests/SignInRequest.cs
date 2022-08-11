using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Requests
{
    public class SignInRequest
    {
        [Required]
        public string? Email { get; set; }        
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}