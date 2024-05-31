using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCrud.Data;
using NetCrud.Dtos;
using NetCrud.Models;
using System.Reflection.Metadata.Ecma335;

namespace NetCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public ArtistController(ApplicationDb db)
        {
            _db=db;
        }

        [HttpGet("get-all")]
        public ActionResult<List<ArtistDto>> GetAll()
        {

            var artist = _db.Artists.Include(a => a.Genre)
                                     .Select(a => new ArtistDto
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         PhotoUrl = a.PhotoUrl,
                                         Genre = a.Genre.Name
                                     }
                                     )
                                     .ToList();

            return artist;
        }

        [HttpGet("get-one/{id}")]
        public ActionResult<ArtistDto> GetOne(int id)
        {

            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var artist = _db.Artists
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    PhotoUrl = a.PhotoUrl,
                    Genre = a.Genre.Name
                })
                .Where(a => a.Id == id)
                .FirstOrDefault();

            if (artist == null)
            {
                return NotFound($"Artista con el id {id} no encontrado");
            }

            return Ok(artist);

        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] ArtistAddEditDto model)
        {
            Console.WriteLine($"model: Name={model.Name}, PhotoUrl={model.PhotoUrl}, Genre={model.Genre}");
            if (model == null)
            {
                return BadRequest("Revisar la peticion");
            }

            if (ArtistNameExists(model.Name))
            {
                return BadRequest("El artista ya existe");
            }

            var fetchedGenre = GetGenreByName(model.Genre);

            if (fetchedGenre == null)
            {
                return BadRequest($"Revisar peticion, nombre de genero no valido");
            }

            var artistToAdd = new Artist
            {
                Name = model.Name.ToLower(),
                Genre = fetchedGenre,
                //GenreId = fetchGenre.Id,
                PhotoUrl= model.PhotoUrl
            };

            _db.Artists.Add(artistToAdd);
            _db.SaveChanges();

            return Ok("Artista Creado");

        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] ArtistAddEditDto model)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            if (model == null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Genre))
            {
                return BadRequest("Datos del artista inválidos.");
            }

            var fetchedArtist = _db.Artists.Find(id);

            if (fetchedArtist == null)
            {
                return NotFound($"Artista no encontrado con el id {id}");
            }

            if (fetchedArtist.Name != model.Name.ToLower() && ArtistNameExists(model.Name)) {
                return BadRequest("El artista ya existe");
            }

            var fetchedGenre = GetGenreByName(model.Genre);

            if (fetchedGenre == null)
            {
                return BadRequest($"Revisar peticion, nombre de genero no valido");
            }

            fetchedArtist.Name = model.Name.ToLower();
            fetchedArtist.Genre = fetchedGenre;
            fetchedArtist.PhotoUrl = model.PhotoUrl;
            _db.SaveChanges();

            return Ok("Artista Actualizado");

        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var fetchedArtist = _db.Artists.Find(id);

            if (fetchedArtist == null)
            {
                return NotFound($"Artista no encontrado con el id {id}");
            }

            _db.Artists.Remove(fetchedArtist);
            _db.SaveChanges();

            return Ok("Artista Eliminado");

        }

        private bool ArtistNameExists(string name)
        {
            return _db.Artists.Any(a => a.Name.ToLower() == name.ToLower());
        }

        private Genre GetGenreByName(string name) {
            return _db.Genres.SingleOrDefault(x=>x.Name.ToLower() == name.ToLower());
        }
    }
}
