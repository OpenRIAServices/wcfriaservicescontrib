using System;
//using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityValidator
{
    public interface IValidationRule<TEntity, TResult> where TResult : class
    {
        event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;
        TResult Result { get; }

        /// <summary>
        /// Validates entity by invoking the validation method of this IValidationRule 
        /// </summary>
        /// <param name="entity"></param>
        void InvokeValidate(TEntity entity);
    }
}
