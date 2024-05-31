using System.ComponentModel.DataAnnotations;

namespace NetCrud.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Artist> Artists { get; set; }
    }
}
