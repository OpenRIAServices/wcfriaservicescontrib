using System.ComponentModel.DataAnnotations;

namespace RiaServicesContrib.DomainServices.Client
{
    /// <summary>
    /// This class defines an entity graph shape that spans all associations in your object
    /// graph that are annotated with the AssociationAttribute.
    /// </summary>
    public class FullEntityGraphShape : DynamicGraphShape
    {
        /// <summary>
        /// Initializes a new instance of the FullEntityGraphShape class
        /// </summary>
        public FullEntityGraphShape() :
            base(prop => prop.IsDefined(typeof(AssociationAttribute), true))
        {
        }
    }
}
