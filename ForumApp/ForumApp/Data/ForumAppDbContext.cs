using ForumApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ForumApp.Data
{
    public class ForumAppDbContext : DbContext
    {

        private Post FirstPost { get; set; }

        private Post SecondPost { get; set; }

        private Post ThirdPost { get; set; }


        public ForumAppDbContext(DbContextOptions<ForumAppDbContext> options)
                :base(options)
        {
            Database.Migrate();

        }

        public DbSet<Post> Posts { get; init; }

        private void SeedPosts()
        {
            FirstPost = new Post()
            {
                Id = 1,
                Title = "First post",
                Content = "Content for my first post"
            };

            SecondPost = new Post()
            {
                Id = 2,
                Title = "Second post",
                Content = "Content for my second post"
            };

            ThirdPost = new Post()
            {
                Id = 3,
                Title = "Third post",
                Content = "Content for my third post"
            };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedPosts();
            modelBuilder
                .Entity<Post>()
                .HasData(FirstPost, SecondPost, ThirdPost);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
