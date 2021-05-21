using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMusic.Mvc.Models
{
    public class Comment
    {
        public Comment()
        {
            CommentTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CommentTime { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
