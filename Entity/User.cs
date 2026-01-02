namespace TicketApp.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        
        // Yeni eklenen alan: Varsayılan olarak herkes 'User' olsun
        public string Role { get; set; } = "User"; 

        public List<TicketPurchase> TicketPurchases { get; set; } = new();
    }
}