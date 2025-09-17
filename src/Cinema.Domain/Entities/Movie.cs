namespace Cinema.Domain.Entities
{
    public class Movie
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int Year { get; set; }
    }
}
