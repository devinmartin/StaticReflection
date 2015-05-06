using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StaticReflection
{
    /// <summary>
    /// Uses Lambda expressions to get MemberInfo objects
    /// </summary>
    public static class LambdaMemberInfo
    {
        /// <summary>
        /// Gets the name of a member defined by a lambda expression as a string 
        /// </summary>
        /// <typeparam name="T">The type of the object in question</typeparam>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <returns>The name of the member</returns>
        public static string MemberNameFromExpression<T>(Expression<Func<T>> expression)
        {
            return MemberInfoFromExpression<T>(expression).Name;
        }

        /// <summary>
        /// Gets the name of a member defined by a lambda expression as a string 
        /// </summary>
        /// <typeparam name="TObject">The type of the object in question</typeparam>
        /// <typeparam name="TReturn">The type of the member being accessed</typeparam>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <returns>The name of the member</returns>
        public static string MemberNameFromExpression<TObject, TReturn>(Expression<Func<TObject, TReturn>> expression)
        {
            return MemberInfoFromExpression<TObject, TReturn>(expression).Name;
        }

        /// <summary>
        /// Gets the member name defined by a lambda expression
        /// </summary>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        public static string MemberNameFromExpression(Expression<Action> expression)
        {
            return MemberInfoFromExpression(expression).Name;
        }

        /// <summary>
        /// Gets the member name defined by a lambda expression
        /// </summary>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <typeparam name="TObject">The type of the object in question</typeparam>
        /// <returns>The member info object</returns>
        public static string MemberNameFromExpression<TObject>(Expression<Action<TObject>> expression)
        {
            return MemberInfoFromExpression<TObject>(expression).Name;
        }

        /// <summary>
        /// Gets the member info defined by a lambda expression
        /// </summary>
        /// <typeparam name="TObject">The type of the object in question</typeparam>
        /// <typeparam name="TReturn">The type of the member being accessed</typeparam>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <returns>The member info object</returns>
        public static MemberInfo MemberInfoFromExpression<TObject, TReturn>(Expression<Func<TObject, TReturn>> expression)
        {
            return ParseExpression(expression.Body);
        }

        /// <summary>
        /// Gets the member info defined by a lambda expression
        /// </summary>
        /// <typeparam name="T">The type of the member being accessed</typeparam>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <returns>The member info object</returns>
        public static MemberInfo MemberInfoFromExpression<T>(Expression<Func<T>> expression)
        {
            return ParseExpression(expression.Body);
        }

        /// <summary>
        /// Gets the member info defined by a lambda expression
        /// </summary>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <returns>The member info object</returns>
        public static MemberInfo MemberInfoFromExpression(Expression<Action> expression)
        {
            return ParseExpression(expression.Body);
        }

        /// <summary>
        /// Gets the member info defined by a lambda expression
        /// </summary>
        /// <param name="expression">The expression that accesses the member. Should be in the format () => object.Member </param>
        /// <typeparam name="TObject">The type of the object in question</typeparam>
        /// <returns>The member info object</returns>
        public static MemberInfo MemberInfoFromExpression<TObject>(Expression<Action<TObject>> expression)
        {
            return ParseExpression(expression.Body);
        }

        private static MemberInfo ParseExpression(Expression expression)
        {
            if (expression is MemberExpression)
            {
                MemberExpression memberAccess = (MemberExpression)expression;
                return memberAccess.Member;
            }
            else if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCall = (MethodCallExpression)expression;
                return methodCall.Method;
            }
            else
                throw new Exception("The lambda expression is invalid. Ensure that the the body returns the result of acessing a field, property, or method call");
        }
    }
}