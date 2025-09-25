namespace Cinema.Domain.Entities
{
    public class User
    {
        public string Uid { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool EmailVerified { get; set; }

        public bool Disabled { get; set; }
    }
}