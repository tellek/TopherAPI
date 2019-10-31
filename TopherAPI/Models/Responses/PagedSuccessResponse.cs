using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopherAPI.Models.Responses
{
    public class PagedSuccessResponse<T>
    {
        public IEnumerable<T> Results { get; set; }
        public bool Cached { get; set; }
    }
}
