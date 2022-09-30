namespace Codat.Bookkeeper.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string HashedPassword { get; set; }
        
        public int TenantId { get; set; }
    }
}
