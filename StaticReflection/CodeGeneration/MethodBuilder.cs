using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace StaticReflection.CodeGeneration
{
    /// <summary>
    /// Base functionality to build methods
    /// </summary>
    public abstract class MethodBuilder<T> : IMethodBuilder<T>
    {
        protected readonly IList<Expression> _expressions;
        protected readonly ParameterExpression _valueParameter;

        protected MethodBuilder()
        {
            this._valueParameter = Expression.Parameter(typeof(T), "value");
            this._expressions = new List<Expression>();
        }

        protected MethodBuilder(IList<Expression> expressions, ParameterExpression valueParameter)
        {
            this._expressions = expressions;
            this._valueParameter = valueParameter;
        }

        public Action<T> Compile()
        {
            var block = Expression.Block(_expressions);

            return Expression.Lambda<Action<T>>(block, _valueParameter).Compile();
        }


        public IMutatorMethodBlock<T> WithMutator()
        {
            return new MutatorMethodBlock<T>(this._expressions, this._valueParameter);
        }
    }
}
