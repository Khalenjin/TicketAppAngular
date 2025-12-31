using TicketApp.Entity;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Concrete.EfCore;

namespace TicketApp.Data
{
    public static class SeedData
{
    public static void SeedDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();

        context.Database.Migrate();

        // Eğer zaten bilet varsa tekrar ekleme
        if (context.Tickets.Any())
            return;

        var plays = new List<(string Name, int Days, decimal Price)>
        {
            ("Hamlet", 5, 120),
            ("Macbeth", 7, 130),
            ("Romeo ve Juliet", 3, 110),
            ("Bir Yaz Gecesi Rüyası", 10, 150),
            ("Kral Lear", 12, 140),
            ("Cimri", 2, 100),
            ("Kürk Mantolu Madonna", 8, 160),
            ("Şair Evlenmesi", 4, 90),
            ("Keşanlı Ali Destanı", 14, 170),
            ("Hastalık Hastası", 6, 115)
        };

        foreach (var play in plays)
        {
            // 1) Her oyun için ayrı salon
            var hall = new Hall
            {
                Name = play.Name + " Salonu",
                RowCount = 10,
                SeatsPerRow = 10
            };

            context.Halls.Add(hall);
            context.SaveChanges();

            // 2) Oyun biletini oluştur
            var ticket = new Ticket
            {
                PlayName = play.Name,
                Price = play.Price,
                Date = DateTime.Now.AddDays(play.Days),
                HallId = hall.Id
            };

            context.Tickets.Add(ticket);
            context.SaveChanges();

            // 3) Koltukları oluştur
            var seats = new List<Seat>();
            for (int row = 1; row <= hall.RowCount; row++)
            {
                for (int num = 1; num <= hall.SeatsPerRow; num++)
                {
                    seats.Add(new Seat
                    {
                        RowNumber = row,
                        SeatNumber = num,
                        HallId = hall.Id
                    });
                }
            }

            context.Seats.AddRange(seats);
            context.SaveChanges();
        }
    }
}

}