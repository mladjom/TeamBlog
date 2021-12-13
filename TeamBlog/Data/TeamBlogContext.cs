using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Models;

namespace TeamBlog.Data
{
    public class TeamBlogContext : IdentityDbContext
    {
        public TeamBlogContext (DbContextOptions<TeamBlogContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Article { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}
