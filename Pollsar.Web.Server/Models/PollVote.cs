using System;

namespace Pollsar.Web.Server.Models
{
    public partial class PollVote
    {
        public long Id { get; set; }
        public long? VoterId { get; set; }
        public virtual User Voter { get; set; }
        public DateTime? VoteDate { get; set; }
        public long? PollChoiceId { get; set; }
        public virtual PollChoice Choice { get; set; }
    }
}