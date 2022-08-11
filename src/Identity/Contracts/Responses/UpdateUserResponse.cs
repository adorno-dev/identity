namespace Identity.Contracts.Responses
{
    public class UpdateUserResponse
    {
        public Guid Id { get; set; }
        public string? Fullname { get; set; }
        public DateTime Birthday { get; set; }

        public UpdateUserResponse(Guid id, string? fullname, DateTime birthday)
        {
            Id = id;
            Fullname = fullname;
            Birthday = birthday;
        }
    }
}