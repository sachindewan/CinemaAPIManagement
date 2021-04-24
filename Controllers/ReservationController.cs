using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly CinemaDbContext _cinemaDbContext;
        public ReservationController(CinemaDbContext cinemaDbContext)
        {
            _cinemaDbContext = cinemaDbContext;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Reservation reservation)
        {
            reservation.ReservationDateTime = DateTime.Now.ToUniversalTime();
            _cinemaDbContext.Reservations.Add(reservation);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        public IActionResult GetReserVation()
        {
            var reserVationData = (from reservation in _cinemaDbContext.Reservations
                                   join m in _cinemaDbContext.Movies on reservation.MovieId equals m.Id
                                   join user in _cinemaDbContext.Users on reservation.UserId equals user.Id
                                   select new
                                   {
                                       ReserVationId = reservation.Id,
                                       Price =reservation.Price,
                                       Time = reservation.ReservationDateTime,
                                       UserName = user.Name,
                                       MovieName = m.Name
                                   }
                                   );
            return Ok(reserVationData);
        }
        [HttpGet("{id}")]
        public IActionResult GetReserVation(int id)
        {
            var reserVationData = (from reservation in _cinemaDbContext.Reservations
                                   join m in _cinemaDbContext.Movies on reservation.MovieId equals m.Id
                                   join user in _cinemaDbContext.Users on reservation.UserId equals user.Id
                                   where reservation.Id ==id
                                   select new
                                   {
                                       ReserVationId = reservation.Id,
                                       Price = reservation.Price,
                                       Time = reservation.ReservationDateTime,
                                       UserName = user.Name,
                                       MovieName = m.Name
                                   }
                                   ).FirstOrDefault();
            return Ok(reserVationData);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReserVation(int id)
        {
            var reserVationData = _cinemaDbContext.Reservations.Find(id);
            if(reserVationData is null)
            {
                return NotFound();
            }
            _cinemaDbContext.Reservations.Remove(reserVationData);
            return Ok();
        }
    }
}
