using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShareMusic.Mvc.Models;

namespace ShareMusic.Mvc.Data
{
    public class ShareMusicMvcContext : IdentityDbContext<ShareMusicMvcUser>
    {
        public ShareMusicMvcContext(DbContextOptions<ShareMusicMvcContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Music> Musics { get; set; }
    }
}
