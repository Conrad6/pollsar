using System;

namespace Pollsar.Web.Server.Models
{
    public partial class PollTag
    {
        public long Id { get; set; }
        public long? PollId { get; set; }
        public virtual Poll Poll { get; set; }
        public long? TagId { get; set; }
        public virtual Tag Tag { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}