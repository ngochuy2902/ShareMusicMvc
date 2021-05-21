using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShareMusic.Mvc.Data;

namespace ShareMusic.Mvc.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ShareMusicMvcContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ShareMusicMvcContext>>()))
            {
                // Look for any movies.
                if (context.Categories.Any())
                {
                    return;   // DB has been seeded
                }
                context.Categories.AddRange(
                    new Category
                    {
                        Name = "tre",
                        Description = "Nhạc trẻ"
                    },
                    new Category
                    {
                        Name = "rap",
                        Description = "Nhạc Rap"
                    },
                    new Category
                    {
                        Name = "bolero",
                        Description = "Nhạc Bolero"
                    },
                    new Category
                    {
                        Name = "edm",
                        Description = "Nhạc EDM"
                    },
                    new Category
                    {
                        Name = "khongloi",
                        Description = "Nhạc không lời"
                    },
                    
                    new Category
                    {
                        Name = "acoustic",
                        Description = "Nhạc Acoustic"
                    }, 
                    new Category
                    {
                        Name = "usuk",
                        Description = "Nhạc US-UK"
                    },
                    new Category
                    {
                        Name = "kpop",
                        Description = "Nhạc K-pop"
                    },
                    new Category
                    {
                        Name = "tinhyeu",
                        Description = "Nhạc tình yêu"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
