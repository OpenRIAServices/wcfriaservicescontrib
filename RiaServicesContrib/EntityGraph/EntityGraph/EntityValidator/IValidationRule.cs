using System;
using System.ComponentModel.Composition;

namespace RIA.EntityValidator
{
    [InheritedExport]
    public interface IValidationRule<TEntity, TResult> where TResult : class
    {
        event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;
        TResult Result { get; }
        /// <summary>
        /// Returns a list of mappings from T to members reachable from T that play a role in the validation rule
        /// </summary>
        ValidationRuleDependencies<TEntity> Signature { get; }

        /// <summary>
        /// Validates entity by invoking the validation method of this IValidationRule 
        /// </summary>
        /// <param name="entity"></param>
        void InvokeValidate(TEntity entity);
    }
}
