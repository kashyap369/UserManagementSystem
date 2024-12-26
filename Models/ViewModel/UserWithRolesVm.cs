namespace AuthApp.Models.ViewModel
{
    public class UserWithRolesVM
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Roles { get; set; } // To hold a comma-separated list of roles
    }
}
