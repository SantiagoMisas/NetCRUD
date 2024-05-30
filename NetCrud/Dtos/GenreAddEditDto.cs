using System.ComponentModel.DataAnnotations;

namespace NetCrud.Dtos
{
    public class GenreAddEditDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
