using System.Runtime.Serialization;

namespace HttpRequest
{
    [DataContract]
    class PriceType
    {
        [DataMember]
        public decimal price_btc { get; set; }

        [DataMember]
        public decimal price_usd { get; set; }

        [DataMember]
        public decimal price_eur { get; set; }

        [DataMember]
        public decimal price_rur { get; set; }

        [DataMember]
        public decimal price_cny { get; set; }

    }

    [DataContract]
    class Price
    {
        [DataMember]
        public bool status { get; set; }

        [DataMember]
        public PriceType data { get; set; }
    }
}