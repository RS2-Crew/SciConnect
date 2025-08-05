namespace IdentityService.DTOs
{
    public class EmailMessage
    {
        public string To { get; set; }      // Can be comma-separated
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
