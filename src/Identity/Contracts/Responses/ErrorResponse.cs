using Microsoft.AspNetCore.Identity;

namespace Identity.Contracts.Responses
{
    public class ErrorResponse
    {
        public string? Message { get; set; }        
        public IEnumerable<IdentityError>? Errors { get; set; }

        public ErrorResponse(IEnumerable<IdentityError> errors)
        {
            Errors = errors;
        }

        public ErrorResponse(string message, IEnumerable<IdentityError>? errors = null)
        {
            Message = message;
            Errors = errors;
        }
    }
}