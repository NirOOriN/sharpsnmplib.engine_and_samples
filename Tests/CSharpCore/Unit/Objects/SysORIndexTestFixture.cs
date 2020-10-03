﻿using Lextm.SharpSnmpLib;
using NooN.SnmpEngine;
using NooN.SnmpEngine.Pipeline;
using Xunit;

namespace Samples.Unit.Objects
{
    public class SysORIndexTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysORIndex(3);
            Assert.Equal("1.3.6.1.2.1.1.9.1.1.3", sys.Variable.Id.ToString());
            Assert.Equal("3", sys.Data.ToString());
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}
