using System.Data.SqlTypes;

namespace QB.Models
{
    public class Client
    {
        public int Npp { get; set; }
        public int IDAPT { get; set; }
        public string Name { get; set; }
        public string ADDR { get; set; }
        public string Director { get; set; }
        public string FioZav { get; set; }
        public string Email { get; set; }
    }
}
