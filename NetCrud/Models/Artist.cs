using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCrud.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public string PhotoUrl { get; set; }

        [ForeignKey("Genre")]
        public int GenreId { get; set; }

        public Genre Genre { get; set; }

        public ICollection<ArtistAlbumBridge> Albums { get; set; }
    }
}
