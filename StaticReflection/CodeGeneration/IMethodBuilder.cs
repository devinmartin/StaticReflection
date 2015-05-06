using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticReflection.CodeGeneration
{

    public interface IMethodBuilder<T>
    {
        /// <summary>
        /// Compiles this instance and returns a delegate that performs the mutation.
        /// </summary>
        /// <returns></returns>
        Action<T> Compile();

        /// <summary>
        /// Composes the current method builder with a mutator.
        /// </summary>
        /// <returns></returns>
        IMutatorMethodBlock<T> WithMutator();
        
    }
}
