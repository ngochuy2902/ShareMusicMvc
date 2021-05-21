using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShareMusic.Mvc.Models;

namespace ShareMusic.Mvc.ViewModels
{
    public class PostViewModels
    {
        public PostViewModels()
        {
            PostTime = DateTime.Now;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImagePath { get; set; }
        public List<IFormFile> MusicPaths { get; set; }
        public DateTime PostTime { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
    }
}
