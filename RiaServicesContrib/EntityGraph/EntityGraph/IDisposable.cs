using System;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> :  IDisposable
    {
        public void Dispose() {
            RemoveNotifyPropertyChangedHandlers();
            RemoveNotifyCollectionChangedHandlers();
        }
    }
}
