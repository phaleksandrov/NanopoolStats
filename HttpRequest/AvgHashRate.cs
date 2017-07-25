using System.Runtime.Serialization;

namespace HttpRequest
{
    [DataContract]
    class AvgHashRateData
    {
        [DataMember]
        public decimal h1 { get; set; }

        [DataMember]
        public decimal h3 { get; set; }

        [DataMember]
        public decimal h6 { get; set; }

        [DataMember]
        public decimal h12 { get; set; }

        [DataMember]
        public decimal h24 { get; set; }
   
    }
    [DataContract]
    class AvgHashRate
    {
        [DataMember]
        public bool status { get; set; }

        [DataMember]
        public AvgHashRateData data { get; set; }
    }
}