
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
         EntitySet<T>  EntitySet { get; }
    }
}
