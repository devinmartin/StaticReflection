using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StaticReflection.CodeGeneration
{
    /// <summary>
    /// Represents a mutator method block.
    /// </summary>
    /// <typeparam name="T">Type to build a mutator for</typeparam>
    public class MutatorMethodBlock<T> : IMutatorMethodBlock<T>
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static IMutatorMethodBlock<T> Create()
        {
            return new MutatorMethodBlock<T>();
        }

        private readonly List<Expression> _expressions = new List<Expression>();
        private readonly ParameterExpression _valueParameter;

        public MutatorMethodBlock()
        {
            this._valueParameter = Expression.Parameter(typeof(T), "value");
        }

        /// <summary>
        /// Adds the re assignment method call for property or field.
        /// Will take the property value, invoke the method with the property value and re-assign the response to the property
        /// </summary>
        /// <param name="memberInfo">The member information for the property or field to mutate</param>
        /// <param name="methodInfo">The static method information that does the mutation</param>
        /// <returns>A reference to this. Optionally used for fluent apis.</returns>
        IMutatorMethodBlock<T> IMutatorMethodBlock<T>.AddReAssignmentMethodCallForPropertyOrField(MemberInfo memberInfo, MethodInfo methodInfo)
        {
            var memberExpression = Expression.PropertyOrField(_valueParameter, memberInfo.Name);
            var invokeExpression = Expression.Call(methodInfo, memberExpression);

            this._expressions.Add(Expression.Assign(memberExpression, invokeExpression));

            return this;
        }

        /// <summary>
        /// Adds the re assignment method call for property or field.
        /// Will take the property value, invoke the method with the property value and re-assign the response to the property
        /// </summary>
        /// <param name="memberInfo">The member information for the property or field to mutate</param>
        /// <param name="methodInfo">The method information that does the mutation</param>
        /// <param name="methodInstance">The instance of the object to use when invoking the mutator method</param>
        /// <returns>A reference to this. Optionally used for fluent apis.</returns>
        IMutatorMethodBlock<T> IMutatorMethodBlock<T>.AddReAssignmentMethodCallForPropertyOrField(MemberInfo memberInfo, MethodInfo methodInfo, object methodInstance)
        {
            ConstantExpression methodInstanceExpression = Expression.Constant(methodInstance);
            var memberExpression = Expression.PropertyOrField(_valueParameter, memberInfo.Name);
            var invokeExpression = Expression.Call(methodInstanceExpression, methodInfo, memberExpression);

            this._expressions.Add(Expression.Assign(memberExpression, invokeExpression));

            return this;
        }
        /// <summary>
        /// Compiles this instance and returns a delegate that performs the mutation.
        /// </summary>
        /// <returns></returns>
        Action<T> IMutatorMethodBlock<T>.Compile()
        {
            var block = Expression.Block(_expressions);

            return Expression.Lambda<Action<T>>(block, _valueParameter).Compile();
        }
    }
}
