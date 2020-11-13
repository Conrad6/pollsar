using System;
using System.Collections.Generic;

namespace Pollsar.Web.Server.Models
{
    public partial class Category
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public virtual ICollection<PollCategory> Polls{ get; }

        public Category(){
            Polls = new HashSet<PollCategory>();
        }
    }
}