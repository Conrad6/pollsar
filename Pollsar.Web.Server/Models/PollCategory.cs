using System;

namespace Pollsar.Web.Server.Models
{
    public partial class PollCategory
    {
        public long Id { get; set; }
        public long? PollId { get; set; }
        public virtual Poll Poll { get; set; }
        public long? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}