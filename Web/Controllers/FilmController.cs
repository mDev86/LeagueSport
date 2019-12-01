using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Web.Models.Domain;
using Web.Models.Domain.Entities;
using WebGrease;
using WebGrease.Css;
using static Web.Models.FilmViewModels;

namespace Web.Controllers
{
    public class FilmController : Controller
    {
        private const int pageSize = 6;
        
        /// <summary>
        /// Главная страница со списком фильмов
        /// </summary>
        /// <param name="page">Номер страницы которую необходимо показать</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(int page = 1)
        {
            int _page = page > 0 ? page - 1 : 0;
            int countFilms = 0; 
            
            List<Film> film = new List<Film>();
            using (EFDBContext context = new EFDBContext())
            {
                countFilms = await context.Films.CountAsync();
                /**
                 * Проверка, что запрашиваемая страница содержит фильмы
                 */
                if (countFilms < _page * pageSize)
                {
                    _page = countFilms/pageSize;
                }
                film = context.Films.Include(f=>f.Owner).OrderByDescending(it => it.Timestamp).Skip(pageSize*_page).Take(pageSize).ToList();
            }

            var model = new ListFilms{
                Count = countFilms,
                SizePage = pageSize,
                CurrentPage = _page+1
            };
            
            foreach (Film _film in film)
            {
                model.Films.Add(new FilmViewModel(_film));
            }
            
            return View(model);
        }

        /// <summary>
        /// Отображение детальной информации о фильме
        /// </summary>
        /// <param name="id">Идентификатор фильма</param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(int id)
        {
            Film film;
            using (EFDBContext context = new EFDBContext())
            {
                film = await context.Films.Include(f=>f.Owner).FirstOrDefaultAsync(it=>it.Id == id);
            }

            if (film == null)
            {
                return HttpNotFound();
            }

            string userId = User?.Identity?.GetUserId() ?? "";

            /**
             * Фильм просматривает тот пользователь, который его добавил
             */
            bool isEditable = film.Owner.Id.Equals(userId);
            
            FilmViewModel model = new FilmViewModel(film, isEditable);

            return View("FilmDetail", model);
        }
        
        [Authorize]
        public ActionResult New()
        {
            return View("NewFilm");
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> New(EditedFilmViewModel model)
        {
            if (!ModelState.IsValid || model.UploadedPoster==null)
            {
                return View("NewFilm");
            }
            Film newFilm = model.ToFilm();
            
            string poster = SavePoster(model.UploadedPoster);
            if (poster == null)
            {
                ModelState.AddModelError("Poster","Невозможно сохранить прикрипленный файл");
                return View("FilmEdit",model);
            }
            newFilm.Poster = poster;
            string currentUserId = User.Identity.GetUserId();
            using (EFDBContext context = new EFDBContext())
            {
                User owner = context.Users.FirstOrDefault(us => us.Id.Equals(currentUserId));
                if (owner == null)
                {
                    ModelState.AddModelError("Owner","Пользователь не найден");
                    return View("NewFilm", model);
                }

                newFilm.Owner = owner;
                context.Films.Add(newFilm);
                await context.SaveChangesAsync();
            }
            
            return RedirectToAction("Detail", new{id = newFilm.Id});
        }
        
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            string userId = User.Identity.GetUserId();
            Film film;
            using (EFDBContext context = new EFDBContext())
            {
                film = await context.Films.FirstOrDefaultAsync(it=>it.Id == id && it.Owner.Id.Equals(userId));
            }

            if (film == null)
            {
                return HttpNotFound();
            }
            
            EditedFilmViewModel model = new EditedFilmViewModel(film);

            return View("FilmEdit", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(int id, EditedFilmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("FilmEdit");
            }
            string currentUserId = User.Identity.GetUserId();
            using (EFDBContext context = new EFDBContext())
            {
                Film film = await context.Films.FirstOrDefaultAsync(it=>it.Id == id && it.Owner.Id.Equals(currentUserId));
                if (film == null)
                {
                    ModelState.AddModelError("Owner","Нет прав на редактирование");
                    return View("FilmEdit", model);
                }

                if (model.UploadedPoster != null)
                {
                    string poster = SavePoster(model.UploadedPoster);
                    if (poster == null)
                    {
                        ModelState.AddModelError("Poster","Невозможно сохранить прикрипленный файл");
                        return View("FilmEdit",model);
                    }

                    film.Poster = poster;
                }
                film.Title = model.Title;
                film.Description = model.Description;
                film.Year = model.Year;
                film.Producer = model.Producer;
                
                await context.SaveChangesAsync();
            }
            
            return RedirectToAction("Detail","Film", new{id});
        }
    
        /// <summary>
        /// Сохранение постера на сервере
        /// </summary>
        /// <param name="file">Файл для сохранения</param>
        /// <returns>Имя сохраненного файла</returns>
        private string SavePoster(HttpPostedFileBase file)
        {
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            };

            /**
             * Имя нового файла в формате GUID и с исходным расширением
             */
            string newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            file.SaveAs(Server.MapPath($"~/Content/Posters/{newFileName}"));
            
            return newFileName;
        }
    }
    
}