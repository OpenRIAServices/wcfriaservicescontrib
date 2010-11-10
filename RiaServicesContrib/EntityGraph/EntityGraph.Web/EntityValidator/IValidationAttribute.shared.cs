using System.Reflection;

namespace RIA.EntityValidator
{
    public interface IValidationAttribute<TEntity, TResult> : 
        IValidationRule<TEntity, TResult> where TResult : class
    {
        void SetPropertyInfo(PropertyInfo propInfo);
    }
}
