using Lextm.SharpSnmpLib;
using NooN.SnmpEngine;
using NooN.SnmpEngine.Pipeline;
using Xunit;

namespace Samples.Unit.Objects
{
    public class SysDescrTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysDescr();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}
