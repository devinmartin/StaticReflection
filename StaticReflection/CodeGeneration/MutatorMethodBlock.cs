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
    public class MutatorMethodBlock<T> : MethodBuilder<T>, IMutatorMethodBlock<T>
    {
        private IList<Expression> list;
        private ParameterExpression parameterExpression;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static IMutatorMethodBlock<T> Create()
        {
            return new MutatorMethodBlock<T>();
        }

        internal MutatorMethodBlock()
        {

        }

        internal MutatorMethodBlock(IList<Expression> list, ParameterExpression parameterExpression)
            : base(list, parameterExpression)
        {
        }

        /// <summary>
        /// Adds the re assignment method call for property or field.
        /// Will take the property value, invoke the method with the property value and re-assign the response to the property
        /// </summary>
        /// <param name="memberInfo">The member information for the property or field to mutate</param>
        /// <param name="methodInfo">The static method information that does the mutation</param>
        /// <returns>A reference to this. Optionally used for fluent apis.</returns>
        IMutatorMethodBlock<T> IMutatorMethodBlock<T>.AddReAssignmentMethodCallForMember(MemberInfo memberInfo, MethodInfo methodInfo)
        {
            MutatorMethodBlock<T>.Validate(memberInfo, methodInfo);

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
        IMutatorMethodBlock<T> IMutatorMethodBlock<T>.AddReAssignmentMethodCallForMember(MemberInfo memberInfo, MethodInfo methodInfo, object methodInstance)
        {
            MutatorMethodBlock<T>.Validate(memberInfo, methodInfo);
            ConstantExpression methodInstanceExpression = Expression.Constant(methodInstance);
            var memberExpression = Expression.PropertyOrField(_valueParameter, memberInfo.Name);
            var invokeExpression = Expression.Call(methodInstanceExpression, methodInfo, memberExpression);

            this._expressions.Add(Expression.Assign(memberExpression, invokeExpression));

            return this;
        }

        private static void Validate(MemberInfo memberInfo, MethodInfo methodInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    MutatorMethodBlock<T>.Validate(((FieldInfo)memberInfo).FieldType, methodInfo);
                    break;
                case MemberTypes.Property:
                    MutatorMethodBlock<T>.Validate(((PropertyInfo)memberInfo).PropertyType, methodInfo);
                    break;
                default:
                    throw new MethodSignatureMismatchException(string.Format("Can't user a member of type {0}", memberInfo.MemberType));
            }
        }

        private static void Validate(Type memberType, MethodInfo methodInfo)
        {
            if (memberType != methodInfo.ReturnType)
                throw new MethodSignatureMismatchException("The return type and the member type don't match");

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1)
                throw new MethodSignatureMismatchException("Must have one parameter matching the member type");

            if (parameters[0].ParameterType != memberType)
                throw new MethodSignatureMismatchException("The parameter doesn't match the member type");
        }
    }
}
