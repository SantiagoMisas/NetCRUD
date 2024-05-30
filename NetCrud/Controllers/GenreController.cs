using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCrud.Data;
using NetCrud.Dtos;
using NetCrud.Models;

namespace NetCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public GenreController(ApplicationDb db)
        {
            _db=db;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var genres = _db.Genres.ToList();
            var toReturn = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = (new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                });

                toReturn.Add(genreDto);
            }
            return Ok(toReturn);
        }

        [HttpGet("get-one/{id}")]
        public IActionResult GetOne(int id)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            try
            {
                var genre = _db.Genres.Find(id);

                if (genre == null)
                {
                    return NotFound($"Genero con el {id} no encontrado.");
                }

                var toReturn = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                };

                return Ok(toReturn);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Se presento un error procesando tu solicitud");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(GenreAddEditDto model)
        {
            if (model == null)
            {
                return BadRequest("Revisar la peticion");
            }

            if (GenreNameExists(model.Name))
            {
                return BadRequest("El genero ya existe");
            }
            try
            {

                var genreToAdd = new Genre
                {
                    Name = model.Name.ToLower()
                };

                _db.Genres.Add(genreToAdd);
                _db.SaveChanges();

                return Ok("Genero Creado");

            } catch (Exception ex) {
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }

        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] GenreAddEditDto model)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var fetchGenre = _db.Genres.Find(id);

            if (fetchGenre == null)
            {
                return NotFound($"Genero no encontrado con el id {id}.");
            }

            if (GenreNameExists(model.Name))
            {
                return BadRequest("El genero ya existe");
            }
            try
            {

                fetchGenre.Name = model.Name.ToLower();
                _db.SaveChanges();

                return Ok("Genero Actualizado");
            }
            catch (Exception ex) {
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var fetchObj = _db.Genres.Find(id);

            if (fetchObj == null)
            {
                return NotFound($"Genero no encontrado con el id {id}.");
            }

            try
            {
                _db.Genres.Remove(fetchObj);
                _db.SaveChanges();
                return Ok("Genero Eliminado");
            }
            catch (Exception ex) {
                return StatusCode(500, "Ocurrio un error al procesar la solicitud.");
            } 
        }

            private bool GenreNameExists(string name)
        {
            var fetchedGenre = _db.Genres.FirstOrDefault(g => g.Name.ToLower() == name.ToLower());

            if(fetchedGenre != null){

                return true;
            }

            return false;
        }
    }
}
