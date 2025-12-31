namespace TicketApp.Entity
{
    public class Ticket
{
    public int Id { get; set; }
    public string PlayName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int HallId { get; set; }
    public Hall? Hall { get; set; }
}

}
