namespace AuthApp.Models.ViewModel
{
    public class RoleAssignVm
    {
        public string Id { get; set; }  
        public string Username { get; set; }
        public string Role {  get; set; } 
        public string SelectedRole { get; set; }
        public List<string> AvailableRoles { get; set; }
    }
}
