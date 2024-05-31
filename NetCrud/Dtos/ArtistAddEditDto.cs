using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NetCrud.Dtos
{
    public class ArtistAddEditDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(60, ErrorMessage ="El nombre del artista no puede superar los {0} caracteres")]
        public string Name { get; set; }

        [JsonPropertyName("photoUrl")]
        public string PhotoUrl { get; set; }

        [Required]
        public string Genre { get; set; }
    }
}
