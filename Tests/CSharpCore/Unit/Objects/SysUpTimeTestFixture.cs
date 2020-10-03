using Lextm.SharpSnmpLib;
using NooN.SnmpEngine;
using NooN.SnmpEngine.Pipeline;
using Xunit;

namespace Samples.Unit.Objects
{
    public class SysUpTimeTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysUpTime();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}
