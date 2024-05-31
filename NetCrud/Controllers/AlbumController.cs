using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCrud.Data;
using NetCrud.Dtos;
using NetCrud.Models;

namespace NetCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public AlbumController(ApplicationDb db)
        {
            _db=db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AlbumAddEditDto model)
        {
            if (model == null)
            {
                return BadRequest("revisar la peticion");
            }

            if (AlbumNameExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                return BadRequest("El album ya existe");
            }

            if (model.ArtistIds == null || model.ArtistIds.Count == 0)
            {
                return BadRequest("Revisar la peticion, debe ser seleccionado al menos un artista");
            }

            var albumToAdd = new Album
            {
                Name = model.Name.ToLower(),
                PhotoUrl = model.Photourl
            };

            _db.Albums.Add(albumToAdd);
            await _db.SaveChangesAsync();

            await AssignArtistsToAlbumAsync(albumToAdd.Id, model.ArtistIds);
            await _db.SaveChangesAsync();

            return Ok("Album Creado");
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<AlbumDto>>> GetAll()
        {
            var albums = await _db.Albums.Select(a=> new AlbumDto
            {
                Id = a.Id,
                Name = a.Name,
                PhotoUrl = a.PhotoUrl,
                Artists = a.Artists.Select(d => new ArtistDto
                {
                    Id = d.Artist.Id,
                    Name = d.Artist.Name,
                    PhotoUrl = d.Artist.PhotoUrl,
                    Genre = d.Artist.Genre.Name
                }).ToList()
            }).ToListAsync();

            return Ok(albums);
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<AlbumDto>> GetOne(int id)
        {
            var album = await _db.Albums
                .Where(d => d.Id == id)
                .Select(a => new AlbumDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        PhotoUrl = a.PhotoUrl,
                        Artists = a.Artists.Select(d => new ArtistDto
                        {
                            Id = d.Artist.Id,
                            Name = d.Artist.Name,
                            PhotoUrl = d.Artist.PhotoUrl,
                            Genre = d.Artist.Genre.Name
                    })
                .ToList()
            }).FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound($"Album con el id {id} no encontrado");
            }

            return Ok(album);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AlbumAddEditDto model)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var fetchedAlbum = await _db.Albums.Include(a => a.Artists).FirstOrDefaultAsync(a => a.Id == id);

            if (fetchedAlbum == null) return NotFound($"Album con el id {id} no encontrado");

            if (fetchedAlbum.Name != model.Name.ToLower() && await AlbumNameExistsAsync(model.Name))
            {
                return BadRequest("El album ya existe");
            }

            foreach (var artist in fetchedAlbum.Artists) {

                var fetchedArtistAlbumBridge = await _db.ArtistAlbumBridges
                                                        .SingleOrDefaultAsync(a => a.ArtistId == artist.ArtistId && a.AlbumId == fetchedAlbum.Id);
                _db.ArtistAlbumBridges.Remove(fetchedArtistAlbumBridge);
            }
            await _db.SaveChangesAsync();

            fetchedAlbum.Name =model.Name.ToLower();
            fetchedAlbum.PhotoUrl = model.Photourl;

            await AssignArtistsToAlbumAsync(fetchedAlbum.Id, model.ArtistIds);
            await _db.SaveChangesAsync();

            return Ok("Album Actualizado");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Revisar la peticion");
            }

            var fetchedAlbum = await _db.Albums.Include(a => a.Artists).FirstOrDefaultAsync(a => a.Id == id);

            if (fetchedAlbum == null) return NotFound($"Album con el id {id} no encontrado");

            foreach (var artist in fetchedAlbum.Artists)
            {

                var fetchedArtistAlbumBridge = await _db.ArtistAlbumBridges
                                                        .SingleOrDefaultAsync(a => a.ArtistId == artist.ArtistId && a.AlbumId == fetchedAlbum.Id);
                _db.ArtistAlbumBridges.Remove(fetchedArtistAlbumBridge);
            }

            await _db.SaveChangesAsync();

            _db.Albums.Remove(fetchedAlbum);
            await _db.SaveChangesAsync();

            return Ok("Album Eliminado");

        }

        private async Task<bool> AlbumNameExistsAsync(string albumName)
        {
            return await _db.Albums.AnyAsync(a => a.Name.ToLower() == albumName.ToLower());
        }

        private async Task AssignArtistsToAlbumAsync(int albumId, List<int> artistIds)
        {

            var artistsIds = artistIds.Distinct().ToList();

            foreach (var artistId in artistsIds)
            {
                var artist = await _db.Artists.FindAsync(artistId);
                if (artist != null)
                {
                    var artistAlbumBridgeToAdd = new ArtistAlbumBridge
                    {
                        AlbumId = albumId,
                        ArtistId = artistId
                    };

                    _db.ArtistAlbumBridges.Add(artistAlbumBridgeToAdd);

                }

            }
        }
    }
}
