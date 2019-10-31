using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopherAPI.CustomExceptions
{
    public class InvalidResourceTypeException : Exception
    {
        public InvalidResourceTypeException()
            : base("Invalid resource type used for the requested action!")
        {
        }

        public InvalidResourceTypeException(Exception inner)
            : base("Invalid resource type used for the requested action!", inner)
        {
        }

        public InvalidResourceTypeException(string message)
            : base(message)
        {
        }

        public InvalidResourceTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
