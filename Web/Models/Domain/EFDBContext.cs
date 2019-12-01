using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Web.Models.Domain.Entities;

namespace Web.Models.Domain
{
    public class EFDBContext: IdentityDbContext<User>
    {
        public EFDBContext():base("FilmStorage")
        {
        }

        public DbSet<Film> Films { get; set; }
        
        public static EFDBContext Create()
        {
            return new EFDBContext();
        }
    }
}