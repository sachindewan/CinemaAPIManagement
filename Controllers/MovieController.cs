using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaAPI.Filters;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private CinemaDbContext _CinemaDbContext { get; }

        public MovieController(CinemaDbContext cinemaDbContext)
        {
            _CinemaDbContext = cinemaDbContext;
        }

        // GET: api/<MovieController>
        [HttpGet]
        public IActionResult Get(string sort,int? pageNumber,int? pageSize)
        {
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;
            var objfromDb = from movie in _CinemaDbContext.Movies
                            select new
                            {
                                MovieName = movie.Name,
                                MovieDescription = movie.Description,
                                Rating = movie.Rating,
                                Genre = movie.Genre
                            };

            switch (sort)
            {
                case "asc":
                    return Ok(objfromDb.Skip((currentPageNumber - 1)* currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                case "desc":
                    return Ok(objfromDb.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
                default:
                    return Ok(objfromDb.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
            }
           
        }

        // GET api/<MovieController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var objfromDb = from movie in _CinemaDbContext.Movies where movie.Id==id
                            select new
                            {
                                MovieName = movie.Name,
                                MovieDescription = movie.Description,
                                Rating = movie.Rating,
                                Genre = movie.Genre
                            };
            return Ok(objfromDb);
        }

        [HttpGet("[action]/{movieName}")]
        public IActionResult FindMovie(string movieName)
        {
            var objfromDb = from movie in _CinemaDbContext.Movies
                            where movie.Name.StartsWith(movieName)
                            select new
                            {
                                MovieName = movie.Name,
                                MovieDescription = movie.Description,
                                Rating = movie.Rating,
                                Genre = movie.Genre
                            };
            return Ok(objfromDb);
        }

        // POST api/<MovieController>
        //[HttpPost]
        //[ServiceFilter(typeof(ModelValidationFilter))]
        //public async Task<IActionResult> Post([FromBody] Movie movie)
        //{
        //    if (ModelState.IsValid)
        //    {
        //       await  _CinemaDbContext.Movies.AddAsync(movie);
        //       return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        [HttpPost]
        [ServiceFilter(typeof(ModelValidationFilter))]
        public async Task<IActionResult> Post([FromForm] Movie movie)
        {
            var guid = Guid.NewGuid();
            var path = Path.Combine("wwwroot", guid + ".jpg");
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await movie.FormFile.CopyToAsync(stream);
            }
            movie.ImageUrl = path.Remove(0,7);
            _CinemaDbContext.Movies.Add(movie);
            await _CinemaDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created,movie.ImageUrl);
        }

        // PUT api/<MovieController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Movie movie)
        {
            try {
                var _movieFromDataBase = await _CinemaDbContext.Movies.FindAsync(id);
                if(_movieFromDataBase is null)
                {
                    return NotFound();
                }
                else
                {
                   _movieFromDataBase.Name = movie.Name;
                   _movieFromDataBase.Genre = movie.Genre;
                   await  _CinemaDbContext.SaveChangesAsync();
                   return Ok();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE api/<MovieController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
