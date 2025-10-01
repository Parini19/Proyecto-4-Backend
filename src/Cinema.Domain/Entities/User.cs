namespace Cinema.Domain.Entities
{
    public class User
    {
        public string Uid { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool EmailVerified { get; set; }

        public bool Disabled { get; set; }

        public string Role { get; set; } // Nueva propiedad para el rol del usuario

        public string Password { get; set; } // Nueva propiedad para la contraseña
    }
}