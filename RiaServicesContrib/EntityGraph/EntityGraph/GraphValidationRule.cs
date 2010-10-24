using System.ComponentModel;
using RIA.EntityValidator;

namespace EntityGraph
{
    public abstract class GraphValidationRule<TEntity, TBase, TResult> : ValidationRule<EntityGraph<TEntity, TBase, TResult>, TResult>
        where TEntity : class, TBase
        where TBase : class, INotifyPropertyChanged
        where TResult : class
    {
    }
}
