
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib
{
    /// <summary>
    /// IExtendedEntity is used to expose properties needed by commonly used RIA Services libraries.
    /// <remarks>
    /// IExtendedEntity should be implemented explicitly.
    /// </remarks>
    /// </summary>
    public interface IExtendedEntity
    {
        /// <summary>
        /// EntitySet that tracks this entity.
        /// </summary>
        EntitySet EntitySet {get; }
    }
    /// <summary>
    /// IExtendedEntity is used to expose properties needed by commonly used RIA Services libraries.
    /// <remarks>
    /// IExtendedEntity should be implemented explicitly.
    /// </remarks>
    /// </summary>
   public interface IExtendedEntity<T> where T : Entity, new()
    {
       /// <summary>
       /// EntitySet that tracks this entity
       /// </summary>
         EntitySet<T>  EntitySet { get; }
    }
}
