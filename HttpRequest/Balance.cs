using System.Runtime.Serialization;

namespace HttpRequest
{
    [DataContract]
    class Balance
    {
        [DataMember]
        public bool status { get; set; }

        [DataMember]
        public decimal data { get; set; }
    }
}