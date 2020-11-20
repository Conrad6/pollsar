using System;
using System.Collections.Generic;
using System.Text;

namespace Pollsar.Shared.Models
{
    public class ResponseEntity<TEntity>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public long TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public string LastPage { get; set; }
        public IEnumerable<TEntity> Content { get; set; }
    }
}
