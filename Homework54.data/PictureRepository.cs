using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework54.data
{
    public class PictureRepository
    {
        private string _connectionString;
        public PictureRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Picture> GetPictures()
        {
            using var context = new PicturesDbContext(_connectionString);
            return context.Pictures.OrderByDescending(p => p.Id).ToList();
        }

        public void AddPicture(Picture picture)
        {
            using var context = new PicturesDbContext(_connectionString);
            context.Pictures.Add(picture);
            context.SaveChanges();
        }
        public void IncrementLikes(Picture picture)
        {
            using var context = new PicturesDbContext(_connectionString);
            Picture likedPicture = new Picture
            {
                Id= picture.Id,
                Title= picture.Title,
                Path=picture.Path,
                Likes=picture.Likes + 1
            };
            context.Pictures.Attach(likedPicture);
            context.Entry(likedPicture).State = EntityState.Modified;
            context.SaveChanges();
        }

        public Picture GetPictureById (int id)
        {
            using var context = new PicturesDbContext(_connectionString);
            return context.Pictures.FirstOrDefault(p => p.Id == id);
        }
    }
}
