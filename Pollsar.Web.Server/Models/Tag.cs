using System;
using System.Collections.Generic;

namespace Pollsar.Web.Server.Models
{
    public partial class Tag
    {
        public long Id{ get; set; }
        public DateTime? DateCreated{ get; set; }
        public DateTime? LastUpdated{ get; set; }
        public virtual ICollection<PollTag> Polls{ get; }

        public Tag(){
            Polls = new HashSet<PollTag>();
        }
    }
}