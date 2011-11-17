using System.ComponentModel;
using System.Linq;

namespace RiaServicesContrib
{
    public partial class EntityGraph<TEntity> : IChangeTracking
    {
        /// <summary>
        /// Resets the entity graph’s state to unchanged by accepting the modifications of all its entities.
        /// </summary>
        public void AcceptChanges()
        {
            foreach(var entity in this.OfType<IChangeTracking>())
            {
                entity.AcceptChanges();
            }
        }
        /// <summary>
        /// Gets the changed status of the entity graph.
        /// </summary>
        public bool IsChanged
        {
            get {
                return this.OfType<IChangeTracking>().Aggregate(false, (isChanged, e) => isChanged | e.IsChanged);            
            }
        }
    }
}
