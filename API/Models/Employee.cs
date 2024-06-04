namespace API.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Passphrase { get; set; }
        public bool IsAppAllowed { get; set; }
        public bool IsAdmin { get; set; }
    }
}
