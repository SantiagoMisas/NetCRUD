using Microsoft.EntityFrameworkCore;
using NetCrud.Models;

namespace NetCrud.Data
{
    public class ApplicationDb: DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {
            
        }


    public DbSet<Genre> Genres { get; set; }
    
    public DbSet<Artist> Artists { get; set; }

    public DbSet<Album> Albums { get; set; }

    public DbSet<ArtistAlbumBridge> ArtistAlbumBridges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) { 

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ArtistAlbumBridge>()
                .HasKey(a => new {a.ArtistId, a.AlbumId});

        modelBuilder.Entity<Artist>()
                    .HasMany(a => a.Albums)
                    .WithOne(a => a.Artist)
                    .HasForeignKey(a => a.ArtistId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                    modelBuilder.Entity<Album>()
                    .HasMany(a => a.Artists)
                    .WithOne(a => a.Album)
                    .HasForeignKey(a => a.AlbumId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        
        }
    }
}
