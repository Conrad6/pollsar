using System;
using System.Collections.Generic;

namespace Pollsar.Web.Server.Models
{
    public partial class PollChoice
    {
        public long Id{ get; set; }
        public string Name{ get; set; }
        public long? PollId{ get; set; }
        public virtual Poll Poll{ get; set; }
        public virtual ICollection<PollVote> Votes{ get; }
        public DateTime? LastUpdated { get; set; }
        public PollChoice(){
            Votes = new HashSet<PollVote>();
        }
    }
}