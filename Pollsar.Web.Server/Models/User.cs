using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Pollsar.Web.Server.Models
{
    public partial class User : IdentityUser<long>
    {
        public string Avatar { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? LastUpdated { get; set; }
        public virtual ICollection<Poll> PollsCreated { get; }
        public User()
        {
            PollsCreated = new HashSet<Poll>();
        }
    }
}