using Microsoft.EntityFrameworkCore;

namespace Info.Pages.Model
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<FollowedTopic> FollowedTopics { get; set; } // Add this
    }

    public class FollowedTopic
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key to associate with the user
        public string Topic { get; set; } // The topic being followed
    }
}
