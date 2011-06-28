
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace RiaServicesContrib
{
    /// <summary>
    /// EntityStateSet is a serializable version of a WCF RIA Services Entity
    /// </summary>
    public class EntityStateSet
    {
        /// <summary>
        /// The original state of the entity
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), DataMember]
        public IDictionary<string, object> OriginalState { get; set; }
        /// <summary>
        /// The current state of the entity if modified
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), DataMember]
        public IDictionary<string, object> ModifiedState { get; set; }
        /// <summary>
        /// True if the entity was deleted
        /// </summary>
        [DataMember]
        public bool IsDelete { get; set; }
        /// <summary>
        /// Original primary key of the entity
        /// </summary>
        [DataMember]
        public object OriginalKey { get; set; }
        /// <summary>
        /// Current primary key of the entity if modified
        /// </summary>
        [DataMember]
        public object ModifiedKey { get; set; }
        /// <summary>
        /// The type of the entity (needed for inheritance support)
        /// </summary>
        [DataMember]
        public string EntityType { get; set; }
    }
}
