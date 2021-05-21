using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMusic.Mvc.Models
{
    public class Post
    {
        public Post()
        {
            PostTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string MusicPath {get;set; }
        public DateTime PostTime { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public ICollection <Music> Musics { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
