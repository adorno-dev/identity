using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }        
    }
}