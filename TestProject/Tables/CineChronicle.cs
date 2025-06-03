using Microsoft.EntityFrameworkCore;

namespace CineChronicle.Tables
{
    public class CineChronicleContext : DbContext
    {
        public DbSet<Authorized> Authorized { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<ContentRecommendation> ContentRecommendation { get; set; }
        public DbSet<DateExit> DateExit { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserContents> UserContents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Server=sql7.freesqldatabase.com;Database=sql7782796;User=sql7782796;Password=Y517c86wGN;Port=3306;CharSet=utf8mb4;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}