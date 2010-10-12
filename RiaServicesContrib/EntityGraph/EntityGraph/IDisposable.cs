using System;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> :  IDisposable where TEntity : Entity
    {
        public void Dispose() {
            RemoveNotifyPropertyChangedHandlers();
            RemoveNotifyCollectionChangedHandlers();
        }
    }
}
