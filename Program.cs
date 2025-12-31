using Microsoft.EntityFrameworkCore;
using TicketApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Razor Views
builder.Services.AddControllersWithViews();

// EF Core SQLite
builder.Services.AddDbContext<TicketContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Sql_connection"]);
});

// Repositories
builder.Services.AddScoped<IUserRepository, EfCoreUserRepository>();
builder.Services.AddScoped<ITicketRepository, EfCoreTicketRepository>();
builder.Services.AddScoped<ITicketPurchaseRepository, EfCoreTicketPurchaseRepository>();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login"; // Razor için
        options.Events.OnRedirectToLogin = context =>
        {
            // API çağrılarında redirect yerine 401 dön
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// CORS: Angular localhost için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Static files (Angular dist olabilir)
app.UseStaticFiles();

// CORS yukarıda tanımlanan politika
app.UseCors("AllowAngular");

// Routing + Auth
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Controllers: Hem API hem Razor
app.MapControllers();

// Razor default (MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}"
);

// Angular fallback
app.MapFallbackToFile("index.html");

// DB Seed (varsa)
SeedData.SeedDatabase(app);

app.Run();
