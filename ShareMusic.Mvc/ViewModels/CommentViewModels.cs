using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareMusic.Mvc.Models;

namespace ShareMusic.Mvc.ViewModels
{
    public class CommentViewModels
    {
        public Post Post { get; set; }
        public string MusicURL { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Music> Musics { get; set; }
    }
}
