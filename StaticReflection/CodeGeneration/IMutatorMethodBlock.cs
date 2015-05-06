using System;
using System.Reflection;

namespace StaticReflection.CodeGeneration
{
    /// <summary>
    /// Represents a mutator method block.
    /// </summary>
    /// <typeparam name="T">Type to build a mutator for</typeparam>
    public interface IMutatorMethodBlock<T>
    {
        /// <summary>
        /// Adds the re assignment method call for property or field.
        /// Will take the property value, invoke the method with the property value and re-assign the response to the property
        /// </summary>
        /// <param name="memberInfo">The member information for the property or field to mutate</param>
        /// <param name="methodInfo">The static method information that does the mutation</param>
        /// <returns>A reference to this. Optionally used for fluent apis.</returns>
        IMutatorMethodBlock<T> AddReAssignmentMethodCallForPropertyOrField(MemberInfo memberInfo, MethodInfo methodInfo);
        /// <summary>
        /// Adds the re assignment method call for property or field.
        /// Will take the property value, invoke the method with the property value and re-assign the response to the property
        /// </summary>
        /// <param name="memberInfo">The member information for the property or field to mutate</param>
        /// <param name="methodInfo">The method information that does the mutation</param>
        /// <param name="methodInstance">The instance of the object to use when invoking the mutator method</param>
        /// <returns>A reference to this. Optionally used for fluent apis.</returns>
        IMutatorMethodBlock<T> AddReAssignmentMethodCallForPropertyOrField(MemberInfo memberInfo, MethodInfo methodInfo, object methodInstance);
        /// <summary>
        /// Compiles this instance and returns a delegate that performs the mutation.
        /// </summary>
        /// <returns></returns>
        Action<T> Compile();
    }
}
