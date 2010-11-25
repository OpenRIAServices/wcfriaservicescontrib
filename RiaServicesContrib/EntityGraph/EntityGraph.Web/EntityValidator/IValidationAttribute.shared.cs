using System.Reflection;

namespace RIA.EntityValidator
{
    public interface IValidationAttribute<TRoot, TResult> : 
        IValidationRule<TRoot, TResult> where TResult : class
    {
        void SetPropertyInfo(PropertyInfo propInfo);
    }
}
