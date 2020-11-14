using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pollsar.Web.Server.Models
{
    public partial class Poll
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long? CreatorId { get; set; }
        [JsonIgnore]
        public virtual User Creator { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public virtual ICollection<PollChoice> Choices { get; }
        public virtual ICollection<PollTag> Tags { get; }
        public virtual ICollection<PollCategory> Categories { get; }
        public virtual ICollection<StaticResource> Images { get; }
        public Poll()
        {
            Images = new HashSet<StaticResource>();
            Choices = new HashSet<PollChoice>();
            Categories = new HashSet<PollCategory>();
            Tags = new HashSet<PollTag>();
        }
    }
}