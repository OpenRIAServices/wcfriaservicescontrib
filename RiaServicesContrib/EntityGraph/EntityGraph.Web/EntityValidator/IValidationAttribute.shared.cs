using System.Reflection;

namespace RIA.EntityValidator
{
    public interface IValidationAttribute<TResult> : 
        IValidationRule<TResult> where TResult : class
    {
        void SetPropertyInfo(PropertyInfo propInfo);
    }
}
