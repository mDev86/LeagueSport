using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Web.Models.Domain.Entities;

namespace Web.Models
{
    public class FilmViewModels
    {
        public class EditedFilmViewModel
        {
            [Required]
            [Display(Name = "Название")]
            public string Title { get; set; }
            
            [Required]
            [Display(Name = "Описание")]
            public string Description { get; set; }
            
            [Required]
            [Display(Name = "Режиссер")]
            public string Producer { get; set; }
            
            [Required]
            [Display(Name = "Год выпуска")]
            [RegularExpression("^[0-9]{4}$", ErrorMessage = "Формат года выпуска неверный")]
            public int Year { get; set; }

            public string PosterUrl { get; set; }

            [Display(Name="Постер")]
            public HttpPostedFileBase UploadedPoster { get; set; }
            
            public Film ToFilm()
            {
                return new Film()
                {
                    Title = this.Title,
                    Description = this.Description,
                    Producer = this.Producer,
                    Year = this.Year,
                    Timestamp = DateTime.Now.ToUniversalTime()
                };
            }

            public EditedFilmViewModel()
            {}

            public EditedFilmViewModel(Film film)
            {
                Title = film.Title;
                Description = film.Description;
                Producer = film.Producer;
                Year = film.Year;

                PosterUrl = $"~/Content/Posters/{film.Poster}";
            }
            
        }
        
        public class FilmViewModel
        {
            public int Id { get; set; }
            
            [Required]
            [Display(Name = "Название")]
            public string Title { get; set; }
            
            [Required]
            [Display(Name = "Описание")]
            public string Description { get; set; }
            
            [Required]
            [Display(Name = "Режиссер")]
            public string Producer { get; set; }
            
            [Required]
            [Display(Name = "Год выпуска")]
            public int Year { get; set; }

            public string PosterUrl { get; set; }
            
            [Display(Name = "Разместил")]
            public string Owner { get; set; }
            public bool IsEditable { get; set; }
            
            public FilmViewModel(){}

            public FilmViewModel(Film film, bool isEditable = false)
            {
                Id = film.Id;
                Title = film.Title;
                Description = film.Description;
                Producer = film.Producer;
                Year = film.Year;

                PosterUrl = $"~/Content/Posters/{film.Poster}";
                
                Owner = film.Owner?.UserName;
                IsEditable = isEditable;
            }

        }

        public class ListFilms
        {
            public int Count { get; set; }

            public int SizePage { get; set; }

            public int CurrentPage { get; set; }

            public List<FilmViewModel> Films { get; set; }

            public ListFilms()
            {
                Films = new List<FilmViewModel>();
            }
        }

    }
}