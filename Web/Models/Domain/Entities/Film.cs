using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations.Model;

namespace Web.Models.Domain.Entities
{
    public class Film
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Producer { get; set; }
        public string Poster { get; set; }

        public DateTime Timestamp { get; set; }
        
        public virtual User Owner { get; set; }
    }
}