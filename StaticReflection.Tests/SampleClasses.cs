using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticReflection.Tests
{
    class InstanceType
    {
        public string NameProperty { get; set; }
        public string NameField;
        internal int InternalField = 0;

        public int MethodInt()
        {
            Assert.Fail("Shouldn't ever actually call the method");

            throw new Exception("uh oh");
        }

        public void MethodVoid()
        {
            Assert.Fail("Shouldn't ever actually call the method");
        }

        public InstanceType GetNewInstance()
        {
            Assert.Fail("Shouldn't ever actually call the method");

            throw new Exception("uh oh");
        }
    }
}
