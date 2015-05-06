﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            mutator.AddReAssignmentMethodCallForPropertyOrField(
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
            mutator.AddReAssignmentMethodCallForPropertyOrField(
                LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(t => t.NameProperty),
                (MethodInfo)LambdaMemberInfo.MemberInfoFromExpression<MutatorMethodBlockTests, string>(t => MutatorMethodBlockTests.MutateString(string.Empty))
            );

            var mutateMethod = mutator.Compile();

            var mutableObject = new InstanceType() { NameProperty = "name" };
            mutateMethod(mutableObject);

            Assert.AreEqual("mutated name", mutableObject.NameProperty);
        }
    }
}
