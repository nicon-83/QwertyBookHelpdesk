namespace QB.Models
{
    public class SiteUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool SendByMail { get; set; }
        public bool SendBySystemOrder { get; set; }
        public int AptekaId { get; set; }
        public long UserId { get; set; }
    }
}
