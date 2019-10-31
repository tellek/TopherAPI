using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopherAPI.Models.Responses
{
    public class SingleSuccessResponse<T>
    {
        public T Result { get; set; }
        public bool Cached { get; set; }
    }
}
