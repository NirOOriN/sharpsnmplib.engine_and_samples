using System;
using System.Runtime.Serialization;

namespace NooN.SnmpEngine.Extended
{
    /// <summary>
    /// VersionCode flags, used for the handlermapping
    /// </summary>
    [DataContract]
    [Flags]
    public enum VersionFlags
    {
        None = 0,
        V1 = 1,
        V2 = 2,
        V3 = 4,
        All = int.MaxValue,
    }
}
