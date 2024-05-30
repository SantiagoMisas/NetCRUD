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
            
        
    }
}
