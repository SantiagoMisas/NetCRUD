using System.ComponentModel.DataAnnotations;

namespace NetCrud.Dtos
{
    public class AlbumAddEditDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Photourl { get; set; }

        public List<int> ArtistIds { get; set; }
    }
}
