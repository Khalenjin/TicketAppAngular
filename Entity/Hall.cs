namespace TicketApp.Entity
{
public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int RowCount { get; set; } 
    public int SeatsPerRow { get; set; } 

    public ICollection<Seat>? Seats { get; set; }
}
}
