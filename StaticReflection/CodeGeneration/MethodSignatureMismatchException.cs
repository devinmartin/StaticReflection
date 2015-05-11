using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticReflection.CodeGeneration
{
    public class MethodSignatureMismatchException : Exception
    {
        public MethodSignatureMismatchException(string message)
            :base(message)
        { }
    }
}
