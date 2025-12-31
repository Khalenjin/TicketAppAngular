using TicketApp.Entity;

namespace TicketApp.Models
{
    public class SelectSeatViewModel
{
    public int TicketId { get; set; }
    public int PersonCount { get; set; }
    public List<Seat> Seats { get; set; } = new();
    public List<int> SelectedSeatIds { get; set; } = new();
    public string SelectedSeatIdsString { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }  // ★ EKLENDİ
}


}
