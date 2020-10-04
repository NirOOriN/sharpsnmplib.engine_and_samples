using Lextm.SharpSnmpLib;
using System.Runtime.Serialization;

namespace NooN.SnmpEngine.Extended
{
    /// <summary>
    /// CommandTypes used for the HandlerMapping
    /// </summary>
    [DataContract]
    public enum CommandType
    {
        None = 0,
        GetRequestPdu = SnmpType.GetRequestPdu,
        GetNextRequestPdu = SnmpType.GetNextRequestPdu,
        ResponsePdu = SnmpType.ResponsePdu,
        SetRequestPdu = SnmpType.SetRequestPdu,
        TrapV1Pdu = SnmpType.TrapV1Pdu,
        GetBulkRequestPdu = SnmpType.GetBulkRequestPdu,
        InformRequestPdu = SnmpType.InformRequestPdu,
        TrapV2Pdu = SnmpType.TrapV2Pdu,
        ReportPdu = SnmpType.ReportPdu,
        All = int.MaxValue,
    }
}
