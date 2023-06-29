using Microsoft.EntityFrameworkCore;
using ReversiRestApi.Models;

namespace ReversiRestApi.DAL
{
    public class ReversiDbContext : DbContext
    {
        public ReversiDbContext(DbContextOptions<ReversiDbContext> options) : base(options) { }
        public DbSet<Game> Games { get; set; }
    }
}
