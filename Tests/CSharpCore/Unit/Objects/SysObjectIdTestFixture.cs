using Lextm.SharpSnmpLib;
using NooN.SnmpEngine;
using NooN.SnmpEngine.Pipeline;
using Xunit;

namespace Samples.Unit.Objects
{
    public class SysObjectIdTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysObjectId();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}
