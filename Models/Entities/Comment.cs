namespace cat_lover_api.Models.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public required string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public User? Author { get; set; }

    }
}
