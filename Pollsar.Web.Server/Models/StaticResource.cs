using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pollsar.Web.Server.Models
{
    public partial class StaticResource
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public long? RefererId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
