using Microsoft.EntityFrameworkCore;
using MyBGList.Models;

namespace MyBGList.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Overriding OnModelCreating method to configure models. HasKey is to configure primary key of [BoardGames_Domains]
        // and [BoardGames_Mechanics] tables.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BoardGames_Domains>()
                .HasKey(i => new { i.BoardGameId, i.DomainId });



            modelBuilder.Entity<BoardGames_Mechanics>()
                .HasKey(i => new { i.BoardGameId, i.MechanicId });
        }
    }
}
