using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMusic.Mvc.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string MusicName { get; set; }
        public string MusicURL { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
