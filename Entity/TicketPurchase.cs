namespace TicketApp.Entity
{
    public class TicketPurchase
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; }

    public int SeatId { get; set; }
    public Seat Seat { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime PurchaseDate { get; set; }
}


}
