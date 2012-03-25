namespace RiaServicesContrib.DomainServices.Client.contrib
{
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public class EntityStateSet
    {
        [DataMember]
        public IDictionary<string, object> OriginalState { get; set; }

        [DataMember]
        public IDictionary<string, object> ModifiedState { get; set; }

        [DataMember]
        public bool IsDelete { get; set; }

        [DataMember]
        public object OriginalKey { get; set; }

        [DataMember]
        public object ModifiedKey { get; set; }

    }
}
