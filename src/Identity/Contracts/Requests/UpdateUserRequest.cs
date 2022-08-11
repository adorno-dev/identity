using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Requests
{
    public class UpdateUserRequest
    {
        [Required]
        public string? Fullname { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
    }
}