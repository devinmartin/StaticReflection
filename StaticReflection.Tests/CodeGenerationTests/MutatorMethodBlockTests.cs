using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticReflection.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StaticReflection.Tests.CodeGenerationTests
{
    [TestClass]
    public class MutatorMethodBlockTests
    {
        static int MutateReturnTypeMismatch(string input)
        {
            return 1;
        }
        static string MutateParameterTypeMismatch(int input)
        {
            return input.ToString();
        }
        static string MutateNoParameters()
        {
            return string.Empty;
        }

        static void MutateVoid(string input)
        { }

        static string MutateExtra(string in1, string in2)
        {
            return in1;
        }
        static string MutateString(string input)
        {
            return "mutated " + input;
        }

        string MutateStringInstance(string input)
        {
            return MutateString(input);
        }

        [TestMethod]
        public void Mutate_Instance()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            mutator.AddReAssignmentMethodCallForMember(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => t.MutateStringInstance(string.Empty)),
                this
            );

            var mutateMethod = mutator.Compile();

            var mutableObject = new InstanceType() { NameProperty = "name" };
            mutateMethod(mutableObject);

            Assert.AreEqual("mutated name", mutableObject.NameProperty);
        }

        [TestMethod]
        public void Mutate_Static()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            mutator.AddReAssignmentMethodCallForMember(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => MutatorMethodBlockTests.MutateString(string.Empty))
            );

            var mutateMethod = mutator.Compile();

            var mutableObject = new InstanceType() { NameProperty = "name" };
            mutateMethod(mutableObject);

            Assert.AreEqual("mutated name", mutableObject.NameProperty);
        }

        [TestMethod]
        public void MutatePassesToComposedMutator()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            mutator.AddReAssignmentMethodCallForMember(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => MutatorMethodBlockTests.MutateString(string.Empty))
            );

            var mutator2 = mutator.WithMutator();
            var mutateMethod = mutator2.Compile();

            var mutableObject = new InstanceType() { NameProperty = "name" };
            mutateMethod(mutableObject);

            Assert.AreEqual("mutated name", mutableObject.NameProperty);
        }

        [TestMethod]
        public void Mutate_Composed()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            mutator.AddReAssignmentMethodCallForMember(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => MutatorMethodBlockTests.MutateString(string.Empty))
            );

            var mutator2 = mutator.WithMutator();
            mutator2.AddReAssignmentMethodCallForMember(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameField),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => t.MutateStringInstance(string.Empty)),
                this
            );


            var mutateMethod = mutator2.Compile();

            var mutableObject = new InstanceType() { NameProperty = "name", NameField = "test" };
            mutateMethod(mutableObject);

            Assert.AreEqual("mutated name", mutableObject.NameProperty);
            Assert.AreEqual("mutated test", mutableObject.NameField);
        }

        [TestMethod]
        public void Mutate_NoParameters()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            Action act = () =>
            {
                mutator.AddReAssignmentMethodCallForMember(
                    LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                    (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests>(t => MutatorMethodBlockTests.MutateNoParameters()));
            };

            act.ShouldThrow<MethodSignatureMismatchException>();
        }

        [TestMethod]
        public void Mutate_Void()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            Action act = () =>
            {
                mutator.AddReAssignmentMethodCallForMember(
                    LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                    (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests>(t => MutatorMethodBlockTests.MutateVoid("")));
            };

            act.ShouldThrow<MethodSignatureMismatchException>();
        }
        [TestMethod]
        public void Mutate_ParameterCountWrong()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            Action act = () =>
            {
                mutator.AddReAssignmentMethodCallForMember(
                    LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                    (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests>(t => MutatorMethodBlockTests.MutateExtra("", "")));
            };

            act.ShouldThrow<MethodSignatureMismatchException>();
        }

        [TestMethod]
        public void Mutate_ParameterTypeMismatch()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            Action act = () =>
            {
                mutator.AddReAssignmentMethodCallForMember(
                    LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                    (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests>(t => MutatorMethodBlockTests.MutateParameterTypeMismatch(1)));
            };

            act.ShouldThrow<MethodSignatureMismatchException>();
        }

        [TestMethod]
        public void Mutate_ReturnTypeMismatch()
        {
            var mutator = MutatorMethodBlock<InstanceType>.Create();
            Action act = () =>
            {
                mutator.AddReAssignmentMethodCallForMember(
                    LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                    (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests>(t => MutatorMethodBlockTests.MutateReturnTypeMismatch("")));
            };

            act.ShouldThrow<MethodSignatureMismatchException>();
        }
    }
}
