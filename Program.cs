using Info.Pages.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register UserDbContext with the DI container
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Login");
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();
// Add session services to your application
builder.Services.AddSession();

var app = builder.Build();

// Enable session middleware
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapRazorPages();

app.Run();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddLogging();










