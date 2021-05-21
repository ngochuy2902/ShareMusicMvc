using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareMusic.Mvc.Models;

namespace ShareMusic.Mvc.ViewModels
{
    public class UserViewModels
    {
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public DateTime JoinTime { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
