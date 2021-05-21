using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ShareMusic.Mvc.Data
{
    // Add profile data for application users by adding properties to the ShareMusicMvcUser class
    public class ShareMusicMvcUser : IdentityUser
    {
        public ShareMusicMvcUser()
        {
            JoinTime = DateTime.Now;
        }

        [PersonalData]
        public DateTime JoinTime { get; set; }
    }
}
