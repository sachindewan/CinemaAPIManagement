using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Data
{
    public class CinemaDbContext:DbContext
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> dbContextOptions):base(dbContextOptions)
        {

        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
