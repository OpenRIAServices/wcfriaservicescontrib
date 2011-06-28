
using System.Runtime.Serialization;
using System.Collections.Generic;
namespace RiaServicesContrib
{
    public class EntityStateSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), DataMember]
        public IDictionary<string, object> OriginalState { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), DataMember]
        public IDictionary<string, object> ModifiedState { get; set; }

        [DataMember]
        public bool IsDelete { get; set; }

        [DataMember]
        public object OriginalKey { get; set; }

        [DataMember]
        public object ModifiedKey { get; set; }

        [DataMember]
        public string EntityType { get; set; }
    }
}
