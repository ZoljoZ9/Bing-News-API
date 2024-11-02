using Info.Pages.Model;
using Microsoft.EntityFrameworkCore;

namespace Info.Pages.Model // Update to match your actual namespace
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
    }
}
