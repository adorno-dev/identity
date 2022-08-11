namespace Identity.Contracts.Responses
{
    public class SignUpResponse
    {
        public string? Message { get; set; }

        public SignUpResponse(string? message)
        {
            this.Message = message;
        }
    }
}