using Homework54.data;
using Microsoft.Extensions.Configuration;

using Homework54.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Homework54.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;
        public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _environment = environment;
        }

        private IWebHostEnvironment _environment;

        public IActionResult Index()
        {
            var repo = new PictureRepository(_connectionString);
            var vm = new IndexViewModel();
            vm.Pictures = repo.GetPictures();
            return View(vm);
        }

        public IActionResult ViewPicture(int pictureId)
        {
            var repo = new PictureRepository(_connectionString);
            if (HttpContext.Session.Get<List<int>>("likedPictures") == null)
            {
                HttpContext.Session.Set("likedPictures", new List<int>());
            }

            var picturesLikedByUser = HttpContext.Session.Get<List<int>>("likedPictures");
            var vm = new ViewPictureViewModel
            {
                Picture = repo.GetPictureById(pictureId),
                PreviouslyLiked = (picturesLikedByUser.Contains(pictureId) ? true: false)
            };
            return View(vm);
        }

        public IActionResult AddPicture()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile picturefile, string pictureName)
        {
            var fileName = $"{Guid.NewGuid()}-{pictureName}{Path.GetExtension(picturefile.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(fullPath, FileMode.CreateNew);
            picturefile.CopyTo(stream);

            var repo = new PictureRepository(_connectionString);
            repo.AddPicture(new Picture
            {
                Title = fileName,
                Path = fullPath,
            });

            return Redirect("/home/index");
        }
        public void AddLike(int pictureId)
        {
            var repo = new PictureRepository(_connectionString);
            Picture p = repo.GetPictureById(pictureId);
            repo.IncrementLikes(p);
            var likedPictures = HttpContext.Session.Get<List<int>>("likedPictures");
            likedPictures.Add(p.Id);
            HttpContext.Session.Set("likedPictures", likedPictures);
        }

        public IActionResult GetLikesCount(int pictureId)
        {
            var repo = new PictureRepository(_connectionString);
            Picture p = repo.GetPictureById(pictureId);
            return Json(p.Likes);
        }
    }
}



public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}