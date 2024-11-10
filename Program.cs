using Info.Pages.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware for serving the Let's Encrypt challenge files
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/.well-known/acme-challenge"))
    {
        await next();
    }
    else
    {
        // Allow the request to continue to the next middleware in the pipeline
        await next();
    }
});


app.UseRouting();

// Enable session middleware
app.UseSession();

// Ensure authentication and authorization middleware are correctly ordered
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
