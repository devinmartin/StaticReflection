using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StaticReflection.Tests
{
    /// <summary>
    /// Tests
    /// </summary>
    [TestClass]
    public class MemberInfoTests
    {
        #region simple existing instance
        [TestMethod]
        public void PropertyName()
        {
            Assert.AreEqual("NameProperty", LambdaMemberInfo.MemberNameFromExpression(() => instance.NameProperty));
        }

        [TestMethod]
        public void FieldName()
        {
            Assert.AreEqual("NameField", LambdaMemberInfo.MemberNameFromExpression(() => instance.NameField));
        }

        [TestMethod]
        public void InternalFieldName()
        {
            Assert.AreEqual("InternalField", LambdaMemberInfo.MemberNameFromExpression(() => instance.InternalField));
        }

        [TestMethod]
        public void MethodReturnName()
        {
            Assert.AreEqual("MethodInt", LambdaMemberInfo.MemberNameFromExpression(() => instance.MethodInt()));
        }

        [TestMethod]
        public void MethodVoidName()
        {
            Assert.AreEqual("MethodVoid", LambdaMemberInfo.MemberNameFromExpression(() => instance.MethodVoid()));
        }


        [TestMethod]
        public void PropertyInfo()
        {
            Assert.AreEqual("NameProperty", LambdaMemberInfo.MemberInfoFromExpression(() => instance.NameProperty).Name);
        }

        [TestMethod]
        public void FieldInfo()
        {
            Assert.AreEqual("NameField", LambdaMemberInfo.MemberInfoFromExpression(() => instance.NameField).Name);
        }

        [TestMethod]
        public void MethodReturnInfo()
        {
            Assert.AreEqual("MethodInt", LambdaMemberInfo.MemberInfoFromExpression(() => instance.MethodInt()).Name);
        }

        [TestMethod]
        public void MethodVoidInfo()
        {
            Assert.AreEqual("MethodVoid", LambdaMemberInfo.MemberInfoFromExpression(() => instance.MethodVoid()).Name);
        }

        #endregion

        #region by type

        [TestMethod]
        public void PropertyName_ByType()
        {
            Assert.AreEqual("NameProperty", LambdaMemberInfo.MemberNameFromExpression<InstanceType, string>(i => i.NameProperty));
        }

        [TestMethod]
        public void FieldName_ByType()
        {
            Assert.AreEqual("NameField", LambdaMemberInfo.MemberNameFromExpression<InstanceType, string>(i => i.NameField));
        }

        [TestMethod]
        public void InternalFieldName_ByType()
        {
            Assert.AreEqual("InternalField", LambdaMemberInfo.MemberNameFromExpression<InstanceType, int>(i => i.InternalField));
        }

        [TestMethod]
        public void MethodReturnName_ByType()
        {
            Assert.AreEqual("MethodInt", LambdaMemberInfo.MemberNameFromExpression<InstanceType, int>(i => i.MethodInt()));
        }

        [TestMethod]
        public void MethodVoidName_ByType()
        {
            Assert.AreEqual("MethodVoid", LambdaMemberInfo.MemberNameFromExpression<InstanceType>(i => i.MethodVoid()));
        }


        [TestMethod]
        public void PropertyInfo_ByType()
        {
            Assert.AreEqual("NameProperty", LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(i => i.NameProperty).Name);
        }

        [TestMethod]
        public void FieldInfo_ByType()
        {
            Assert.AreEqual("NameField", LambdaMemberInfo.MemberInfoFromExpression<InstanceType, string>(i => i.NameField).Name);
        }

        [TestMethod]
        public void MethodReturnInfo_ByType()
        {
            Assert.AreEqual("MethodInt", LambdaMemberInfo.MemberInfoFromExpression<InstanceType, int>(i => i.MethodInt()).Name);
        }

        [TestMethod]
        public void MethodVoidInfo_ByType()
        {
            Assert.AreEqual("MethodVoid", LambdaMemberInfo.MemberInfoFromExpression<InstanceType>(i => i.MethodVoid()).Name);
        }

        #endregion

        [TestMethod]
        public void ArrayAccess()
        {
            InstanceType[] array = null;
            Assert.AreEqual("NameField", LambdaMemberInfo.MemberInfoFromExpression(() => array[0].NameField).Name);
        }

        [TestMethod]
        public void LongChain()
        {
            InstanceType[] array = null;
            Assert.AreEqual("NameProperty", LambdaMemberInfo.MemberNameFromExpression(() => array[0].GetNewInstance().NameProperty));
        }

        /// <summary>
        /// Ensure that nothing is actually run
        /// </summary>
        private InstanceType instance
        {
            get
            {
                Assert.Fail("Shouldn't actually invoke the property");

                throw new Exception("Shouldn't ever get here");
            }
        }
    }
}
